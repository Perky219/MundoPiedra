using UnityEngine;

public class MinionMeleeAI : MonoBehaviour
{
    [Header("Refs")]
    public Animator animator;
    public Transform target;          // normalmente el Player

    [Header("Movimiento")]
    public float detectionRange = 15f;
    public float stopDistance = 2.5f;
    public float moveSpeed = 3f;

    [Header("Ataque")]
    public float attackCooldown = 1.5f;
    public int damage = 10; // ajusta según la vida del jugador

    float nextAttackTime = 0f;

    void Start()
    {
        if (!animator)
            animator = GetComponent<Animator>();

        if (!target)
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!target) return;

        float dist = Vector3.Distance(transform.position, target.position);

        // Fuera de rango → idle
        if (dist > detectionRange)
        {
            animator.CrossFade("IdleNormal", 0.1f);
            return;
        }

        // Dirección hacia el jugador (solo en XZ)
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;

        // si el vector es casi cero, no intentamos mover ni rotar
        if (dir.sqrMagnitude < 0.0001f)
        {
            if (dist <= stopDistance)
            {
                animator.CrossFade("Attack01", 0.1f);
                TryAttack();
            }
            else
            {
                animator.CrossFade("IdleNormal", 0.1f);
            }
            return;
        }

        dir.Normalize();

        if (dist > stopDistance)
        {
            // Caminar hacia el jugador
            animator.CrossFade("WalkFWD", 0.1f);
            transform.position += dir * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(dir);
        }
        else
        {
            // Atacar cuando está cerca
            animator.CrossFade("Attack01", 0.1f);
            transform.rotation = Quaternion.LookRotation(dir);
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;

        if (!target) return;

        var hp = target.GetComponent<Health>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
        }
    }
}
