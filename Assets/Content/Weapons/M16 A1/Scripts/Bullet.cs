using UnityEngine;

public class SimpleBullet : MonoBehaviour
{
    [Header("Bullet Setup")]
    public float baseDamage = 1f;
    public float speed = 10f;
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
            float finalDamage = baseDamage * PlayerStats.Instance.damageMultiplier;
            targetHealth.TakeDamage(Mathf.RoundToInt(finalDamage));

            if (!PlayerStats.Instance.hasPiercing)
                Destroy(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }
}
