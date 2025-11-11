using UnityEngine;

public class FinalBossController : MonoBehaviour
{
    public Animator animator;
    public Transform player;

    public float detectionRange = 20f;
    public float moveSpeed = 3f;

    void Start()
    {
        if (!animator)
            animator = GetComponent<Animator>();

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            // Anim: caminar
            animator.SetBool("Idle", false);
            animator.SetBool("IsWalking", true);

            // Movimiento físico hacia el jugador
            Vector3 dir = (player.position - transform.position);
            dir.y = 0f;
            dir.Normalize();

            transform.position += dir * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(dir);
        }
        else
        {
            // Anim: idle
            animator.SetBool("IsWalking", false);
            animator.SetBool("Idle", true);
        }
    }
}
