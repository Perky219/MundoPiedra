using UnityEngine;

public class SimpleBullet : MonoBehaviour
{
    public float baseDamage = 1f;
    public float speed = 40f;
    public float lifeTime = 5f;
    public GameObject destroyEffect;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Health targetHealth = other.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(Mathf.RoundToInt(baseDamage));
        }

        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
