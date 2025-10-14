using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Health : MonoBehaviour
{
    [Header("Salud")]
    public int maxHealth = 10;
    public int currentHealth;

    [Header("Muerte / Animación")]
    public Animator animator;               // Asignar si quieres animación
    public string deathTrigger = "Die";     // Nombre del Trigger en tu Animator
    public bool destroyOnDeath = true;
    public float destroyDelay = 3f;

    [Header("Eventos")]
    public UnityEvent onDamaged;
    public UnityEvent onDeath;

    public bool IsDead { get; private set; }

    void Awake()
    {
        currentHealth = Mathf.Max(1, maxHealth);
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        currentHealth -= Mathf.Abs(amount);
        onDamaged?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;

        // Animación de muerte
        if (animator != null && !string.IsNullOrEmpty(deathTrigger))
            animator.SetTrigger(deathTrigger);

        // Desactiva lógicas de IA / movimiento típicas
        var ai = GetComponent<MonoBehaviour>();
        // Mejor desactiva componentes concretos si los tienes:
        var archer = GetComponent<EnemigoArcher>();
        if (archer) archer.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb) { rb.velocity = Vector3.zero; rb.isKinematic = true; }

        onDeath?.Invoke();

        if (destroyOnDeath)
            Destroy(gameObject, destroyDelay);
    }
}
