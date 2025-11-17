using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class WeaponShoot : MonoBehaviour
{
    [Header("Weapon Setup")]
    public GameObject bulletPrefab;
    public Transform muzzlePoint;

    [HideInInspector]
    public float bulletSpeed = 20f;

    void Update()
    {
        if (GameState.isCardUIOpen)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            Shoot();
    }

    void Shoot()
    {
        if (muzzlePoint == null)
        {
            Debug.LogError("No hay un MuzzlePoint asignado!");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);

        SimpleBullet sb = bullet.GetComponent<SimpleBullet>();
        if (sb != null)
        {
            sb.speed = bulletSpeed + PlayerStats.Instance.extraBulletSpeed;
        }

        AudioSource audio = muzzlePoint.GetComponent<AudioSource>();
        if (audio != null)
            audio.Play();
    }
}
