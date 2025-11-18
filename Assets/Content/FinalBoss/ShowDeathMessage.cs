using UnityEngine;
using TMPro;

public class ShowDeathMessage : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    
    [TextArea]
    public string mensaje;

    void OnDisable()
    {
        if (messageText != null)
        {
            messageText.text = mensaje;
            messageText.gameObject.SetActive(true);
        }
    }
}
