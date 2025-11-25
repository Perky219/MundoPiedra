using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalEndingController : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public GameObject finalImageObject;

    [TextArea]
    public string[] lines;

    public KeyCode advanceKey = KeyCode.Space;

    private int index = 0;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        dialoguePanel.SetActive(true);
        finalImageObject.SetActive(false);

        dialogueText.text = lines[0];
    }

    void Update()
    {
        if (Input.GetKeyDown(advanceKey))
        {
            index++;

            if (index >= lines.Length)
            {
                dialoguePanel.SetActive(false);
                finalImageObject.SetActive(true);
                return;
            }

            dialogueText.text = lines[index];
        }
    }
}
