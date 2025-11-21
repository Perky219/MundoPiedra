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
            SceneManager.LoadScene("GameOverScene");
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}