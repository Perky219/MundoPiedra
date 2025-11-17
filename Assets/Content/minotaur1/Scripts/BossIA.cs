using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BossPhaseController))]
public class BossAI : MonoBehaviour
{
    public Transform target;

    [Header("Movimiento")]
    public float walkSpeed = 1.5f;
    public float runSpeed  = 3.5f;
    public float turnSpeed = 3f;

    [Header("Combate")]
    public float attackRange = 2.2f;
    public float attackCooldown = 2.0f;
    public float attackActiveTime = 0.5f;

    [Header("Pathfinding")]
    public float repathInterval = 0.3f;

    NavMeshAgent agent;
    Animator anim;
    BossPhaseController phaseController;

    static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
    static readonly int AttackHash    = Animator.StringToHash("Attack");

    float nextRepathTime;
    float nextAttackTime;
    public bool IsAttackActive { get; private set; }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        phaseController = GetComponent<BossPhaseController>();
    }

    void Start()
    {
            // Si no se asignó manualmente en el inspector,
    // intenta buscar al jugador por la tag "Player"
        if (!target)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p)
            {
                target = p.transform;
            }
            else
            {
                Debug.LogWarning("BossAI: No se encontró ningún objeto con tag 'Player'.");
            }
        }
        if (anim) anim.applyRootMotion = false;

        if (agent)
        {
            agent.stoppingDistance = Mathf.Max(0.1f, attackRange - 0.3f);

            if (!agent.isOnNavMesh &&
                NavMesh.SamplePosition(transform.position, out var hit, 2f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }

    void Update()
    {
        if (!target)
        {
            SetMoving(false);
            SetRunning(false);
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);

        // Elegir velocidad según fase (camina o corre)
        if (phaseController != null && phaseController.CanRun())
        {
            agent.speed = runSpeed;
            SetRunning(true);
        }
        else
        {
            agent.speed = walkSpeed;
            SetRunning(false);
        }

        if (dist > attackRange)
        {
            // Moverse hacia el jugador
            agent.isStopped = false;

            if (Time.time >= nextRepathTime)
            {
                nextRepathTime = Time.time + repathInterval;
                agent.SetDestination(target.position);
            }

            SetMoving(agent.velocity.sqrMagnitude > 0.01f);
        }
        else
        {
            // Detenerse y atacar
            agent.isStopped = true;
            SetMoving(false);

            // Mirar al jugador
            Vector3 to = target.position - transform.position;
            to.y = 0f;
            if (to.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(to),
                    turnSpeed * Time.deltaTime
                );
            }

            // Ataque básico cuerpo a cuerpo
            if (Time.time >= nextAttackTime)
            {
                StartCoroutine(DoAttackWindow());
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    System.Collections.IEnumerator DoAttackWindow()
    {
        anim?.SetTrigger(AttackHash);
        IsAttackActive = true;
        yield return new WaitForSeconds(attackActiveTime);
        IsAttackActive = false;
    }

    void SetMoving(bool m)
    {
        if (anim) anim.SetBool(IsMovingHash, m);
    }

    void SetRunning(bool r)
    {
        if (anim) anim.SetBool(IsRunningHash, r);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.35f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
