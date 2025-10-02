using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 5f;          // Tiempo de vida antes de destruirse
    public AudioClip impactSound;        // Sonido de impacto
    public GameObject impactEffect;      // Efecto visual al chocar (opcional)

    private void Start()
    {
        // Destruir la bala automáticamente tras cierto tiempo
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Reproducir sonido de impacto
        if (impactSound != null)
        {
            AudioSource.PlayClipAtPoint(impactSound, transform.position);
        }

        // Instanciar efecto de impacto
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
