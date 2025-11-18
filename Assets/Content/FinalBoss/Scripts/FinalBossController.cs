using UnityEngine;

public class FinalBossController : MonoBehaviour
{
    [Header("Refs")]
    public Animator animator;
    public Transform player;

    [Header("Movimiento")]
    public float detectionRange = 20f;
    public float moveSpeed = 3f;

    [Header("Invocación de minions")]
    public GameObject minionPrefab;      // prefab de TurtleShell
    public Transform[] summonPoints;     // puntos hijos del Hunter
    public float firstSummonDelay = 3f;  // tiempo antes de la primera invocación
    public float summonCooldown = 10f;   // tiempo entre invocaciones
    public int minionsPerWave = 2;       // cuántos minions por invocación
    public int maxMinionsAlive = 4;      // límite total en escena

    [Header("Ataque cuerpo a cuerpo")]
    public float attackRange = 2.5f;     // distancia para “golpear”
    public float attackCooldown = 1.0f;  // tiempo entre golpes
    public int damagePerHit = 1;         // 1 de daño → 5 golpes si el player tiene 5HP

    float nextSummonTime = 0f;
    int currentMinions = 0;

    float nextAttackTime = 0f;

    void Start()
    {
        if (!animator)
            animator = GetComponent<Animator>();

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // primera invocación dentro de X segundos
        nextSummonTime = Time.time + firstSummonDelay;
    }

    void Update()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            // --- Movimiento hacia el jugador ---
            animator.SetBool("Idle", false);
            animator.SetBool("IsWalking", true);

            Vector3 dir = player.position - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.0001f)
            {
                dir.Normalize();
                transform.position += dir * moveSpeed * Time.deltaTime;
                transform.rotation = Quaternion.LookRotation(dir);
            }

            // --- Ataque si está lo bastante cerca ---
            if (dist <= attackRange)
            {
                TryAttackPlayer();
            }

            // --- Invocación de minions ---
            HandleSummon();
        }
        else
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("Idle", true);
        }
    }

    void TryAttackPlayer()
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;

        if (!player) return;

        var hp = player.GetComponent<Health>();
        if (hp != null)
        {
            hp.TakeDamage(damagePerHit);
            Debug.Log($"Boss golpea al jugador. Daño: {damagePerHit}");
        }
    }

    void HandleSummon()
    {
        if (minionPrefab == null) return;
        if (summonPoints == null || summonPoints.Length == 0) return;
        if (Time.time < nextSummonTime) return;
        if (currentMinions >= maxMinionsAlive) return;

        nextSummonTime = Time.time + summonCooldown;

        for (int i = 0; i < minionsPerWave; i++)
        {
            if (currentMinions >= maxMinionsAlive) break;

            int index = i % summonPoints.Length;
            Transform spawnPoint = summonPoints[index];

            Instantiate(minionPrefab, spawnPoint.position, spawnPoint.rotation);
            currentMinions++;
        }
    }

    public void MinionDied()
    {
        currentMinions = Mathf.Max(0, currentMinions - 1);
    }
}
