using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public float damageMultiplier = 1f;
    public float extraBulletSpeed = 0f;
    public bool hasPiercing = false;
    public bool hasMultiShot = false;
    public Sprite damageSprite;
    public Sprite multishotSprite;
    public Sprite piercingSprite;
    public Sprite speedSprite;

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

        var ui = FindObjectOfType<UpgradesUIManager>();
        if (ui != null && damageSprite != null)
            ui.AddUpgrade(damageSprite);
    }

    public void AddBulletSpeed(float amount)
    {
        extraBulletSpeed += amount;
        Debug.Log("Nueva velocidad extra de bala: " + extraBulletSpeed);

        var ui = FindObjectOfType<UpgradesUIManager>();
        if (ui != null && speedSprite != null)
            ui.AddUpgrade(speedSprite);
    }

    public void EnablePiercing()
    {
        hasPiercing = true;
        Debug.Log("Piercing activado");

        var ui = FindObjectOfType<UpgradesUIManager>();
        if (ui != null && piercingSprite != null)
            ui.AddUpgrade(piercingSprite);
    }

    public void EnableMultiShot()
    {
        hasMultiShot = true;
        Debug.Log("MultiShot activado");

        var ui = FindObjectOfType<UpgradesUIManager>();
        if (ui != null && multishotSprite != null)
            ui.AddUpgrade(multishotSprite);
    }
}
