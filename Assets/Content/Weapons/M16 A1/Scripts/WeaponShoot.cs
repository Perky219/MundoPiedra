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

        if (PlayerStats.Instance.hasMultiShot)
        {
            ShootMultishot();
        }
        else
        {
            ShootSingle();
        }

        AudioSource audio = muzzlePoint.GetComponent<AudioSource>();
        if (audio != null)
            audio.Play();
    }

    void ShootSingle()
    {
        CreateBullet(muzzlePoint.rotation);
    }

    void ShootMultishot()
    {
        float angle = 10f; // puedes cambiar esto si quieres más separación

        // Bala central
        CreateBullet(muzzlePoint.rotation);

        // Bala izquierda
        CreateBullet(Quaternion.Euler(0, -angle, 0) * muzzlePoint.rotation);

        // Bala derecha
        CreateBullet(Quaternion.Euler(0, angle, 0) * muzzlePoint.rotation);
    }

    void CreateBullet(Quaternion rotation)
    {
        GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, rotation);

        SimpleBullet sb = bullet.GetComponent<SimpleBullet>();
        if (sb != null)
        {
            sb.speed = bulletSpeed + PlayerStats.Instance.extraBulletSpeed;
        }
    }
}
