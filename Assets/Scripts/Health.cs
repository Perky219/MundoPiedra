using UnityEngine;

public class Health : MonoBehaviour
{
    public int hp = 3;
    public void TakeHit(int amount = 1) {
        hp -= amount;
        if (hp <= 0) Destroy(gameObject);
    }
}
