using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float delay = 3f;                // Tiempo total antes de explotar
    public float radius = 5f;               // Radio de daño/fuerza
    public float explosionForce = 700f;     // Fuerza de la onda expansiva
    public GameObject explosionEffect;      // Prefab del efecto visual
    public AudioClip explosionSound;        // Sonido de explosión

    private bool hasExploded = false;

    void Start()
    {
        StartCoroutine(ExplosionSequence());
    }

    IEnumerator ExplosionSequence()
    {
        // Espera hasta un segundo antes del estallido
        yield return new WaitForSeconds(delay - 1f);

        if (explosionSound != null)
        {
            // Creamos un AudioSource configurado correctamente
            GameObject soundObj = new GameObject("ExplosionSound");
            soundObj.transform.position = transform.position;
            AudioSource src = soundObj.AddComponent<AudioSource>();

            src.clip = explosionSound;
            src.volume = 1f;                // Volumen real (100%)
            src.spatialBlend = 1f;          // 1 = completamente 3D
            src.rolloffMode = AudioRolloffMode.Linear;
            src.minDistance = 1f;           // Distancia mínima antes de bajar volumen
            src.maxDistance = 500f;         // Que se escuche desde lejos
            src.Play();

            Destroy(soundObj, explosionSound.length); // limpiar después
        }

        // Espera el último segundo antes de explotar visualmente
        yield return new WaitForSeconds(1f);

        Explode();
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(effect, 5f);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, radius);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
