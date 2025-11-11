using UnityEngine;

public class SimpleBullet : MonoBehaviour
{
    public float baseDamage = 10f;
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

    void OnCollisionEnter(Collision collision)
    {
        float finalDamage = baseDamage * PlayerStats.Instance.damageMultiplier;

        Health targetHealth = collision.gameObject.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(Mathf.RoundToInt(finalDamage));
        }

        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
