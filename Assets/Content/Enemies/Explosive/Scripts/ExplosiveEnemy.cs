using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ExplosiveEnemy : MonoBehaviour
{
    [Header("Detección y combate")]
    public float activateRange = 6f;
    public bool fightStarted = false;

    [Header("Ataque explosivo")]
    public float chaseSpeed = 2.5f;
    public float explosionDistance = 1.5f; // distancia para iniciar cuenta regresiva
    public float explosionDelay = 2f;

    private float explodeTimer = 0f;
    private bool exploding = false;

    [Header("Explosión")]
    public float explosionRadius = 3f;
    public GameObject explosionEffect;

    [Header("Animator")]
    public string speedParam = "Speed";

    Animator anim;
    Rigidbody rb;
    DialoguePopup dialogue;
    GameObject player;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");

        GameObject popup = FindObjectOfType<DialoguePopup>(true)?.gameObject;
        if (popup != null) dialogue = popup.GetComponent<DialoguePopup>();

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (!fightStarted)
        {
            CheckPlayerDistanceForStart();
            return;
        }

        if (!exploding)
        {
            ChasePlayer();
        }
        else
        {
            RunExplosionTimer();
        }
    }

    void CheckPlayerDistanceForStart()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.transform.position);

        if (dist <= activateRange)
        {
            fightStarted = true;

            if (dialogue != null)
            {
                dialogue.Show("Al fin veré si la profecía era cierta.");
                Invoke(nameof(HideDialogue), 2.5f);
            }
        }
    }

    void ChasePlayer()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.transform.position);

        // Si está dentro de rango de explosión → detenerse e iniciar cuenta
        if (dist <= explosionDistance)
        {
            exploding = true;
            explodeTimer = explosionDelay;

            anim.SetFloat(speedParam, 0f);
            return;
        }

        // Seguir al jugador
        Vector3 dir = (player.transform.position - transform.position).normalized;
        Vector3 move = dir * chaseSpeed * Time.deltaTime;

        rb.MovePosition(transform.position + move);
        anim.SetFloat(speedParam, chaseSpeed);
    }

    void RunExplosionTimer()
    {
        explodeTimer -= Time.deltaTime;

        if (explodeTimer <= 0f)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (explosionEffect) Instantiate(explosionEffect, transform.position, Quaternion.identity);

        foreach (var c in Physics.OverlapSphere(transform.position, explosionRadius))
        {
            if (c.CompareTag("Player"))
            {
                Debug.Log("Jugador dañado por explosión");
            }
        }

        Destroy(gameObject);
    }

    void HideDialogue()
    {
        if (dialogue != null)
            dialogue.Hide();
    }
}
