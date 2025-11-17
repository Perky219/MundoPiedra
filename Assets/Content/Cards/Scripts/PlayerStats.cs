using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public float damageMultiplier = 1f;
    public float fireRateMultiplier = 1f;
    public float piercingMultiplier = 1f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void IncreaseDamage(float amount)
    {
        damageMultiplier += amount;
        Debug.Log("Nuevo multiplicador de daño: " + damageMultiplier);
    }

    public void IncreaseFireRate(float amount)
    {
        fireRateMultiplier += amount;
        Debug.Log("Nuevo multiplicador de cadencia: " + fireRateMultiplier);
    }

    public void IncreasePiercing(float amount)
    {
        piercingMultiplier += amount;
        Debug.Log("Nuevo multiplicador de penetración: " + piercingMultiplier);
    }
}
