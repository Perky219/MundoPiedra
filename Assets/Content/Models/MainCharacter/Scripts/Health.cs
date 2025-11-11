using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        if (currentHP < 0) currentHP = 0;

        UpdateUI();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHP / maxHP;
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto");

        if (CompareTag("Player"))
        {
            // Por si el juego estaba en pausa
            Time.timeScale = 1f;

            // Volver al menú principal (asegúrate que la escena se llama así)
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            // Enemigos u otros objetos
            gameObject.SetActive(false);
        }
    }
}

