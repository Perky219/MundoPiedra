using UnityEngine;

public class PiercingCard : MonoBehaviour
{
    public void ApplyCard()
    {
        PlayerStats.Instance.EnablePiercing();
        Debug.Log("Carta de piercing aplicada");

        // Cerrar UI
        transform.parent.parent.gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        GameState.isCardUIOpen = false;
    }
}
