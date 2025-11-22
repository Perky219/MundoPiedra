using UnityEngine;

public class MultipleBulletsCard : MonoBehaviour
{
    public void ApplyCard()
    {
        PlayerStats.Instance.EnableMultiShot();

         // Cerrar UI
        transform.parent.parent.gameObject.SetActive(false);
        GameState.isCardUIOpen = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("Multiple Bullets Card Applied: Multi-shot enabled.");
    }
}
