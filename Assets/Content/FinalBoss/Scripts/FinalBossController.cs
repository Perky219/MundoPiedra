using System.Collections;
using UnityEngine;
using TMPro;

public class FinalBossController : MonoBehaviour
{
    [Header("Refs")]
    public Animator animator;
    public Transform player;

    [Header("Movimiento")]
    public float detectionRange = 20f;
    public float moveSpeed = 3f;

    [Header("Invocación de minions")]
    public GameObject minionPrefab;
    public Transform[] summonPoints;
    public float firstSummonDelay = 3f;
    public float summonCooldown = 10f;
    public int minionsPerWave = 2;
    public int maxMinionsAlive = 4;

    [Header("Intro de diálogo")]
    public Canvas dialogueCanvas;
    public TMP_Text dialogueText;
    public float timeBetweenLines = 2.5f;

    [Header("Ataque cuerpo a cuerpo")]
    public float attackRange = 2.5f;     // distancia para “golpear”
    public float attackCooldown = 1.0f;  // tiempo entre golpes
    public int damagePerHit = 1;         // 1 de daño → 5 golpes si el player tiene 5HP

    float nextSummonTime = 0f;
    int currentMinions = 0;

    bool introPlayed = false;
    bool introPlaying = false;

    void Start()
    {
        if (!animator)
            animator = GetComponent<Animator>();

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        nextSummonTime = Time.time + firstSummonDelay;

        if (dialogueCanvas != null)
            dialogueCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (!introPlayed && dist <= detectionRange)
        {
            introPlayed = true;
            StartCoroutine(PlayIntroDialogue());
        }

        if (introPlaying)
            return;

        if (dist <= detectionRange)
        {
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

            HandleSummon();
        }
        else
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("Idle", true);
        }
    }

    IEnumerator PlayIntroDialogue()
    {
        introPlaying = true;

        if (dialogueCanvas != null)
            dialogueCanvas.gameObject.SetActive(true);

        string[] lines =
        {
            "Llevo mucho tiempo esperando, insecto...",
            "¿De verdad crees que vas a vencerme? JA, JA, JA... qué inepto.",
            "Bien... vamos a verlo."
        };

        foreach (string line in lines)
        {
            if (dialogueText != null)
                dialogueText.text = line;

            yield return new WaitForSeconds(timeBetweenLines);
        }

        if (dialogueCanvas != null)
            dialogueCanvas.gameObject.SetActive(false);

        introPlaying = false;
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

            GameObject minion = Instantiate(minionPrefab, spawnPoint.position, spawnPoint.rotation);
            currentMinions++;

            var minionAI = minion.GetComponent<MinionMeleeAI>();
            if (minionAI != null)
                minionAI.target = player;
        }
    }

    public void MinionDied()
    {
        currentMinions = Mathf.Max(0, currentMinions - 1);
    }
}
