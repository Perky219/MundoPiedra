using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    public Animator animator;
    private bool isPlayerNearby = false;
    private bool isOpen = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isOpen)
        {
            animator.SetTrigger("Open");
            isOpen = true;
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
