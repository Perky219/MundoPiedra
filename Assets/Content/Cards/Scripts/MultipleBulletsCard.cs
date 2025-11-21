using UnityEngine;

public class MultipleBulletsCard : MonoBehaviour
{
    public void ApplyCard()
    {
        PlayerStats.Instance.EnableMultiShot();
    }
}
