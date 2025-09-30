using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("Refs")]
    public Transform target;          // arrastra Target o Player
    public NavMeshAgent agent;        // se autoasignará en Reset
    public Animator animator;         // se autoasignará en Reset

    [Header("Combate")]
    public float attackRange = 1.2f;
    public int attacksToDo = 3;
    public float attackCooldown = 0.9f; // tiempo entre golpes

    bool isAttacking, isDead;

    void Reset() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start() {
        if (agent != null) agent.stoppingDistance = attackRange * 0.95f;
        if (animator != null) animator.applyRootMotion = false; // camina con NavMeshAgent
    }

    void Update()
    {
        if (isDead || target == null || agent == null) return;

        // Seguir al objetivo
        agent.SetDestination(target.position);

        // Anim caminar
        if (animator != null)
            animator.SetBool("IsWalking", agent.velocity.sqrMagnitude > 0.01f);

        // ¿En rango de ataque?
        if (!isAttacking && Vector3.Distance(transform.position, target.position) <= attackRange + 0.05f)
            StartCoroutine(AttackThenDie());
    }

    IEnumerator AttackThenDie()
    {
        isAttacking = true;
        if (agent != null) agent.isStopped = true;

        int remaining = attacksToDo;
        while (remaining-- > 0)
        {
            if (animator != null) animator.SetTrigger("Attack");

            // (opcional) aplicar daño a mitad del golpe
            yield return new WaitForSeconds(attackCooldown * 0.5f);
            var h = target.GetComponent<Health>();
            if (h != null) h.TakeHit(1);

            yield return new WaitForSeconds(attackCooldown * 0.5f);
        }

        // Morir
        isDead = true;
        if (animator != null) animator.SetTrigger("Die");

        yield return new WaitForSeconds(2f); // deja terminar la anim de muerte
        if (agent != null) agent.enabled = false;
        Destroy(gameObject, 3f);
    }
}
