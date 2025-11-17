using UnityEngine;
using UnityEngine.UI; // mostar vida, próximo sprint
using UnityEngine.SceneManagement;   //  NUEVO

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

        if (CompareTag("Player"))
        {
            Time.timeScale = 1f;

            // Ir a la escena de Game Over
            SceneManager.LoadScene("GameOverScene"); 
        }
        else
        {
            // Enemigos u otros objetos siguen igual que antes
            gameObject.SetActive(false);
        }
    }
}
