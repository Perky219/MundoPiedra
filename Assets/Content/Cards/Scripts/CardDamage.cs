using UnityEngine;

public class CardDamage : MonoBehaviour
{
    public float bonusDamage = 1f;

    public void ApplyCard()
    {
        PlayerStats.Instance.IncreaseDamage(bonusDamage);

        // Ocultar la UI
        transform.parent.parent.gameObject.SetActive(false);

        // Restaurar control
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        GameState.isCardUIOpen = false;

        Debug.Log("Carta de daño aplicada");
    }
}
