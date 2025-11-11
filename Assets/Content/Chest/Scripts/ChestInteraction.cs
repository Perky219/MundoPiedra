using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    public Animator animator;
    public GameObject cardsUI;

    private bool isPlayerNearby = false;
    private bool isOpen = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isOpen)
        {
            animator.SetTrigger("Open");
            isOpen = true;

            // Mostrar la UI de cartas
            if (cardsUI != null)
            {
                cardsUI.SetActive(true);

                // Mostrar y liberar el cursor
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = false;
    }
}
