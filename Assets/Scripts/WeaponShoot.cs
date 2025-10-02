using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class WeaponShoot : MonoBehaviour
{
    public GameObject bulletPrefab;       // Prefab de la bala
    public Transform muzzlePoint;         // La boquilla del rifle
    public float bulletSpeed = 40f;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) // Click izquierdo
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Dirección desde la cámara hacia adelante
        Vector3 shootDirection = mainCamera.transform.forward;

        // Instanciar la bala en la boquilla del rifle
        GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, Quaternion.identity);

        // Agregar velocidad en la dirección de la cámara
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = shootDirection * bulletSpeed;

        // Reproducir sonido (AudioSource en el prefab)
        AudioSource audio = bullet.GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.Play();
        }
    }
}
