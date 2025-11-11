using UnityEngine;
using System.Collections;

public class FinalBossController : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animator;
    public Transform player;
    public GameObject meleeMinionPrefab;
    public Transform[] summonPoints;

    [Header("Stats del Boss")]
    public float detectionRange = 15f;
    public float attackRange = 3f;
    public float moveSpeed = 2f;
    public float summonCooldown = 10f;
    public int health = 10;

    private float summonTimer;
    private bool isDead = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        summonTimer = summonCooldown;
    }

    void Update()
    {
        if (isDead || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Movimiento hacia el jugador
        if (dist < detectionRange && dist > attackRange)
        {
            MoveTowardsPlayer();
        }
        // Ataque
        else if (dist <= attackRange)
        {
            AttackPlayer();
        }
        else
        {
            Idle();
        }

        // Invocación de enemigos
        summonTimer -= Time.deltaTime;
        if (summonTimer <= 0f)
        {
            SummonEnemies();
            summonTimer = summonCooldown;
        }
    }

    void MoveTowardsPlayer()
    {
        animator.SetBool("IsWalking", true);
        animator.SetBool("Idle", false);

        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.LookAt(player);
    }

    void Idle()
    {
        animator.SetBool("IsWalking", false);
        animator.SetBool("Idle", true);
    }

    void AttackPlayer()
    {
        animator.SetBool("IsWalking", false);
        animator.SetTrigger("StandAttack");
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;
        animator.SetTrigger("GotDamage");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("Death", true);
        Debug.Log("El jefe ha muerto");
    }

    void SummonEnemies()
    {
        if (meleeMinionPrefab == null || summonPoints.Length == 0) return;

        for (int i = 0; i < summonPoints.Length; i++)
        {
            Instantiate(meleeMinionPrefab, summonPoints[i].position, Quaternion.identity);
        }

        Debug.Log("Boss invoca enemigos cuerpo a cuerpo");
    }
}
