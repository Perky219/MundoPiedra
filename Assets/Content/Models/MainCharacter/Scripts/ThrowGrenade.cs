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

    public float throwDelay = 2.03f;

    private Animator animator;

    // UI
    public GrenadeUI grenadeUI;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (grenadeUI != null)
            grenadeUI.SetThrowing(false);
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

            if (grenadeUI != null)
                grenadeUI.SetThrowing(true);

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

        if (grenadeUI != null)
            grenadeUI.SetThrowing(false);
    }
}
