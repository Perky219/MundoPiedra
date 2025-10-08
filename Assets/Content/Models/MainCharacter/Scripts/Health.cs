using UnityEngine;
using UnityEngine.UI; // mostar vida, próximo sprint

public class Health : MonoBehaviour
{
    [Header("Vida")]
    public int maxHP = 15;
    public int currentHP;

    [Header("UI opcional")]
    public Slider healthBar; 

    void Start()
    {
        currentHP = maxHP;
        UpdateUI();
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        UpdateUI();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (healthBar)
        {
            healthBar.value = (float)currentHP / maxHP;
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto");
        // Aquí puedes reproducir animación o desactivar controles
        gameObject.SetActive(false);
    }
}

