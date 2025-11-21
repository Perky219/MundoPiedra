using UnityEngine;
using TMPro;

public class DialoguePopup : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public CanvasGroup canvasGroup;

    void Awake()
    {
        Hide(); // que arranque oculto
    }

    public void Show(string text)
    {
        if (dialogueText != null)
            dialogueText.text = text;

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }
}