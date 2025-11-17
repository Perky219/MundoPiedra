using UnityEngine;

public class FireSpeedCard : MonoBehaviour
{
    public float bonus = 10f; // Cada carta aumenta +10

    public void ApplyCard()
    {
        PlayerStats.Instance.AddBulletSpeed(bonus);

        // Cerrar UI
        transform.parent.parent.gameObject.SetActive(false);
        GameState.isCardUIOpen = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("Carta FireSpeed aplicada");
    }
}
