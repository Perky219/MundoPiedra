using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Rangos y tiempos")]
    public float attackRange = 1.1f;
    public float attackCooldown = 1.0f;
    public float repathInterval = 0.2f;

    private NavMeshAgent agent;
    private Animator animator;

    private float nextPathTime;
    private float nextAttackTime;

    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (!agent.isOnNavMesh && NavMesh.SamplePosition(transform.position, out var hit, 2f, NavMesh.AllAreas))
            agent.Warp(hit.position);

        if (animator != null)
            animator.applyRootMotion = false;
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // Si está lejos, perseguir
        if (distance > attackRange)
        {
            agent.isStopped = false;

            if (Time.time >= nextPathTime)
            {
                nextPathTime = Time.time + repathInterval;
                agent.SetDestination(target.position);
            }

            animator.SetBool(IsWalkingHash, agent.velocity.sqrMagnitude > 0.01f);
        }
        else // Está en rango de ataque
        {
            agent.isStopped = true;
            animator.SetBool(IsWalkingHash, false);

            // Mirar al jugador suavemente
            Vector3 dir = target.position - transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);

            // Atacar con cooldown
            if (Time.time >= nextAttackTime)
            {
                animator.SetTrigger(AttackHash);
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void DealDamage()
{
    if (target == null) return;

    float distance = Vector3.Distance(transform.position, target.position);
    if (distance <= attackRange + 0.2f) // margen de seguridad
    {
        Health hp = target.GetComponent<Health>();
        if (hp != null)
        {
            hp.TakeDamage(1); // daño por golpe
            Debug.Log(" Zombie golpeó al jugador");
        }
    }
}
}

