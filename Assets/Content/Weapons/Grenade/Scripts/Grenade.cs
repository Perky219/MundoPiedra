using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float delay = 3f;                // Tiempo antes de explotar
    public float radius = 5f;               // Radio de daño/fuerza
    public float explosionForce = 700f;     // Fuerza de la onda expansiva
    public GameObject explosionEffect;      // Prefab del efecto visual
    public AudioClip explosionSound;        // Sonido de explosión

    private float countdown;
    private bool hasExploded = false;

    void Start()
    {
        countdown = delay;
    }

    void Update()
    {
        countdown -= Time.deltaTime;

        if (countdown <= 0f && !hasExploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;

        // Efecto visual
       if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(effect, 5f); 
        }

        // Sonido
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // Detección de objetos cercanos
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, radius);
            }
        }

        // Destruir granada
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
