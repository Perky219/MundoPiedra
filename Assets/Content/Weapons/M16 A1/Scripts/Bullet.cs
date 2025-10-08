using UnityEngine;

public class SimpleBullet : MonoBehaviour
{
    public float speed = 40f;
    public float lifeTime = 5f;
    public float damage = 10f; // Daño que inflige el proyectil
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
        // Buscar si el objeto golpeado tiene componente de salud
        Health targetHealth = collision.gameObject.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }

        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
