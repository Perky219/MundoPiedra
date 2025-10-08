using UnityEngine;

public class SimpleBullet : MonoBehaviour
{
    public float speed = 40f;
    public float lifeTime = 5f;
    public float damage = 25f; // sigue siendo float por flexibilidad
    public GameObject destroyEffect;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Intenta obtener el componente de salud
        Health targetHealth = collision.gameObject.GetComponent<Health>();
        if (targetHealth != null)
        {
            // Convierte el daño a int antes de pasarlo al método
            targetHealth.TakeDamage(Mathf.RoundToInt(damage));
        }

        // Efecto visual al destruirse
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
