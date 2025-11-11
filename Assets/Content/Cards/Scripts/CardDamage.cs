using UnityEngine;

public class CardDamage : MonoBehaviour
{
    public float bonusDamage = 0.5f;

    public void ApplyCard()
    {
        PlayerStats.Instance.IncreaseDamage(bonusDamage);
        Debug.Log("Carta de daño aplicada");

        // Ocultar el canvas de cartas
        transform.parent.parent.gameObject.SetActive(false);

        // Ocultar el cursor y volver al control del jugador
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
