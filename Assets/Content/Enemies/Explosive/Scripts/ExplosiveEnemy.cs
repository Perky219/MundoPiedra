using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ExplosiveEnemy : MonoBehaviour
{
    [Header("Movimiento de prueba")]
    public float moveSpeed = 2.5f;
    public float stepDistance = 1f;

    [Header("Explosión")]
    public float explosionDelay = 2f;   // <-- ahora explota a los 2s
    public float explosionRadius = 3f;
    public GameObject explosionEffect;

    [Header("Animator")]
    public string speedParam = "Speed";

    Animator anim;
    Rigidbody rb;

    float timer;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb   = GetComponent<Rigidbody>();
    }

    void Start()
    {
        timer = explosionDelay;  // cuenta atrás de 2 segundos
        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    void Update()
    {
        // Simulación de caminar recto (solo para ver animación antes de explotar)
        Vector3 step = transform.forward * moveSpeed * Time.deltaTime;

        if (rb && !rb.isKinematic) rb.MovePosition(transform.position + step);
        else                       transform.position += step;

        anim.SetFloat(speedParam, moveSpeed);

        // Temporizador
        timer -= Time.deltaTime;
        if (timer <= 0f) Explode();
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
}
