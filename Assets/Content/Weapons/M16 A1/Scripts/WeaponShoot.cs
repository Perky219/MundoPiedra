using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class WeaponShoot : MonoBehaviour
{
    [Header("Weapon Setup")]
    public GameObject bulletPrefab;   // Prefab de la bala
    public Transform muzzlePoint;     // Boquilla del rifle
    public float bulletSpeed = 40f;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) // Click izquierdo
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (muzzlePoint == null)
        {
            Debug.LogError("No hay un MuzzlePoint asignado!");
            return;
        }

        // Instanciar la bala en la posición y rotación del muzzle
        GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);

        // Darle velocidad en la dirección hacia la que apunta el muzzle
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = muzzlePoint.forward * bulletSpeed;
        }

        // Sonido desde el arma (muzzle)
        AudioSource audio = muzzlePoint.GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.Play();
        }
    }
}
