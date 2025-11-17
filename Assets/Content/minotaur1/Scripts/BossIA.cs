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
    public float runSpeed = 3.5f;
    public float turnSpeed = 3f;

    [Header("Ataque")]
    public float attackRange = 2.2f;
    public float attackCooldown = 2f;
    public float attackActiveTime = 0.5f; // ventana del golpe

    [Header("Pathfinding")]
    public float repathInterval = 0.3f;

    NavMeshAgent agent;
    Animator anim;
    BossPhaseController phaseController;

    static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
    static readonly int AttackHash = Animator.StringToHash("Attack");

    float nextRepathTime;
    float nextAttackTime;

    public bool IsAttackActive { get; private set; }
    public bool hasDealtDamageThisSwing = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        phaseController = GetComponent<BossPhaseController>();

        // Buscar player automáticamente si no fue asignado
        if (!target)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) target = p.transform;
        }
    }

    void Start()
    {
        if (anim) anim.applyRootMotion = false;

        if (agent && !agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out var hit, 2f, NavMesh.AllAreas))
                agent.Warp(hit.position);
        }
        agent.stoppingDistance = attackRange - 0.3f;
    }

    void Update()
    {
        if (!target) return;

        float dist = Vector3.Distance(transform.position, target.position);

        // CAMINAR O CORRER SEGÚN FASE
        if (phaseController.CanRun())
        {
            agent.speed = runSpeed;
            anim.SetBool(IsRunningHash, true);
        }
        else
        {
            agent.speed = walkSpeed;
            anim.SetBool(IsRunningHash, false);
        }

        // MOVIMIENTO
        if (dist > attackRange)
        {
            agent.isStopped = false;

            if (Time.time >= nextRepathTime)
            {
                nextRepathTime = Time.time + repathInterval;
                agent.SetDestination(target.position);
            }

            anim.SetBool(IsMovingHash, agent.velocity.sqrMagnitude > 0.01f);
        }
        else
        {
            // DETENERSE Y ATACAR
            agent.isStopped = true;
            anim.SetBool(IsMovingHash, false);

            // rotación suave hacia el jugador
            Vector3 dir = target.position - transform.position;
            dir.y = 0;
            if (dir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);

            if (Time.time >= nextAttackTime)
            {
                StartCoroutine(DoAttackWindow());
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    // Ventana de ataque
    System.Collections.IEnumerator DoAttackWindow()
    {
        anim.SetTrigger(AttackHash);
        IsAttackActive = true;
        hasDealtDamageThisSwing = false;

        yield return new WaitForSeconds(attackActiveTime);

        IsAttackActive = false;
    }
}
