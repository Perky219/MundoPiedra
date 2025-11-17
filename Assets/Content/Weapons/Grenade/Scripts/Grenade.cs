using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float delay = 3f;
    public float radius = 5f;
    public float explosionForce = 700f;
    public GameObject explosionEffect;   // tu VFX_Fire_01_Big
    public AudioClip explosionSound;

    [Header("Damage Settings")]
    public int explosionDamage = 5;
    public int fireDamagePerSecond = 1;
    public float fireDuration = 5f; // duración del fuego después de explotar

    private bool hasExploded = false;

    void Start()
    {
        StartCoroutine(ExplosionSequence());
    }

    IEnumerator ExplosionSequence()
    {
        yield return new WaitForSeconds(delay - 1f);

        if (explosionSound != null)
        {
            GameObject soundObj = new GameObject("ExplosionSound");
            soundObj.transform.position = transform.position;
            AudioSource src = soundObj.AddComponent<AudioSource>();
            src.clip = explosionSound;
            src.volume = 1f;
            src.spatialBlend = 1f;
            src.rolloffMode = AudioRolloffMode.Linear;
            src.minDistance = 1f;
            src.maxDistance = 500f;
            src.Play();
            Destroy(soundObj, explosionSound.length);
        }

        yield return new WaitForSeconds(1f);
        Explode();
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // efecto visual de explosión
        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(effect, fireDuration + 1f);
        }

        // daño y fuerza de explosión
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider nearby in colliders)
        {
            Health hp = nearby.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(explosionDamage);
                Debug.Log($"Explosión -5 a {hp.gameObject.name}");
            }

            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, radius);
            }
        }

        // daño por fuego persistente
        StartCoroutine(FireDamageOverTime());

        Destroy(gameObject, 0.2f); // limpia la granada original
    }

    IEnumerator FireDamageOverTime()
    {
        float elapsed = 0f;

        while (elapsed < fireDuration)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider nearby in colliders)
            {
                Health hp = nearby.GetComponent<Health>();
                if (hp != null)
                {
                    hp.TakeDamage(fireDamagePerSecond);
                    Debug.Log($"Fuego -1 a {hp.gameObject.name}");
                }
            }

            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
