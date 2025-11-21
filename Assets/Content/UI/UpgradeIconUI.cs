using UnityEngine;
using UnityEngine.UI;

public class UpgradeIconUI : MonoBehaviour
{
    public Image iconImage;

    public void Setup(Sprite sprite)
    {
        iconImage.sprite = sprite;
    }
}