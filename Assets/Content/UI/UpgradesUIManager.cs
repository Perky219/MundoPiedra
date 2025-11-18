using UnityEngine;

public class UpgradesUIManager : MonoBehaviour
{
    public Transform upgradesPanel;       // donde van los iconos
    public GameObject upgradeIconPrefab;  // tu UpgradeIcon

    public void AddUpgrade(Sprite sprite)
    {
        GameObject iconObj = Instantiate(upgradeIconPrefab, upgradesPanel);
        iconObj.GetComponent<UpgradeIconUI>().Setup(sprite);
    }
}
