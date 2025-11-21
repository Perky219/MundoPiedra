using UnityEngine;
using UnityEngine.UI;

public class GrenadeUI : MonoBehaviour
{
    public Image overlay;

    public void SetThrowing(bool isThrowing)
    {
        if (overlay != null)
            overlay.enabled = isThrowing;
    }
}
