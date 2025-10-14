using UnityEngine;
using System.Collections;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ThrowGrenade : MonoBehaviour
{
    public GameObject grenadePrefab;
    public Transform throwPoint;
    public float throwForce = 10f;

    [Tooltip("Tiempo (segundos) antes de lanzar la granada después de iniciar la animación")]
    public float throwDelay = 2.03f; // Tiempo del frame 61

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current.qKey.wasPressedThisFrame)
#else
        if (Input.GetKeyDown(KeyCode.Q))
#endif
        {
            animator.SetTrigger("Throw");
            StartCoroutine(ThrowGrenadeAfterDelay());
        }
    }

    private IEnumerator ThrowGrenadeAfterDelay()
    {
        yield return new WaitForSeconds(throwDelay);

        GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(throwPoint.forward * throwForce, ForceMode.VelocityChange);
        }
    }
}
