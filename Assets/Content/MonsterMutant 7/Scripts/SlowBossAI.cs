using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SlowBossAI : MonoBehaviour
{
    public Transform target;

    [Header("Combate")]
    public float attackRange = 2.2f;
    public float attackCooldown = 2.0f;
    public float attackActiveTime = 0.5f; // tiempo en que el golpe cuenta
    public float turnSpeed = 3f;          // gira más lento para sensación pesada

    [Header("Repathing")]
    public float repathInterval = 0.3f;   // repite pathfinding cada cierto tiempo

    NavMeshAgent agent;
    Animator anim;

    static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    static readonly int AttackHash   = Animator.StringToHash("Attack");

    float nextRepathTime, nextAttackTime;
    public bool IsAttackActive { get; private set; }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim  = GetComponent<Animator>();

        if (anim) anim.applyRootMotion = false;
        if (agent)
        {
            //agent.speed = 0.1f; // lento
            //agent.acceleration = 0.8f;
            agent.stoppingDistance = Mathf.Max(0.1f, attackRange - 0.3f);

            if (!agent.isOnNavMesh && NavMesh.SamplePosition(transform.position, out var hit, 2f, NavMesh.AllAreas))
                agent.Warp(hit.position);
        }
    }

    void Update()
    {
        if (!target) { SetMoving(false); return; }

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > attackRange)
        {
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
            agent.isStopped = true;
            SetMoving(false);

            // mirar al jugador lentamente
            Vector3 to = target.position - transform.position; 
            to.y = 0f;
            if (to.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(to), turnSpeed * Time.deltaTime);

            // atacar con cooldown
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.35f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

