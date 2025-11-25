using UnityEngine;

public class MinionMeleeAI : MonoBehaviour
{
    [Header("Refs")]
    public Animator animator;
    public Transform target;          // normalmente el Player

    [Header("Movimiento")]
    public float moveSpeed = 3f;
    public float stopDistance = 2.5f; // distancia para dejar de caminar y atacar

    [Header("Ataque")]
    public float attackCooldown = 1.2f;
    public int damage = 1;            // daño que hace cada minion

    float nextAttackTime = 0f;

    void Start()
    {
        if (!animator)
            animator = GetComponent<Animator>();

        // backup por si acaso
        if (!target)
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!target) return;

        float dist = Vector3.Distance(transform.position, target.position);

        // dirección solo en XZ
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        dir.Normalize();

        if (dist > stopDistance)
        {
            // Caminar hacia el jugador
            if (animator) animator.CrossFade("WalkFWD", 0.1f);

            transform.position += dir * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(dir);
        }
        else
        {
            // Atacar cuando está lo bastante cerca
            if (animator) animator.CrossFade("Attack01", 0.1f);
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;

        if (!target) return;

        // MISMO SISTEMA QUE EL BOSS: Health del Player
        Health hp = target.GetComponent<Health>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
            Debug.Log($"{name} golpea al jugador y hace {damage} de daño");
        }
    }
}
