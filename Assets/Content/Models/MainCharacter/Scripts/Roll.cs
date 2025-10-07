using UnityEngine;

public class Roll : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;

    [Header("Roll Settings")]
    public float rollSpeed = 6f;
    public float rollDuration = 0.8f;
    public float rollCooldown = 0.2f;
    public float maskBlendSpeed = 6f;
    public float rotationBlendSpeed = 6f;
    public float returnBlendDuration = 0.25f; // tiempo para blend al idle

    private bool isRolling = false;
    private bool restoringMask = false;
    private bool blendingBack = false;

    private Vector3 rollDirection;
    private float rollTimer = 0f;
    private float upperBodyWeight = 1f;
    private Quaternion startRotation;
    private Quaternion endRotation;
    private float blendTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        int upperBodyLayer = animator.GetLayerIndex("UpperBody");

        // Suaviza el blend de la máscara
        if (restoringMask)
        {
            upperBodyWeight = Mathf.Lerp(upperBodyWeight, 1f, Time.deltaTime * maskBlendSpeed);
            animator.SetLayerWeight(upperBodyLayer, upperBodyWeight);

            if (upperBodyWeight > 0.98f)
            {
                restoringMask = false;
                animator.SetLayerWeight(upperBodyLayer, 1f);
            }
        }

        // Durante el roll
        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            controller.Move(rollDirection * rollSpeed * Time.deltaTime);

            if (rollTimer <= 0f)
                EndRoll();

            return;
        }

        // Durante el blend de retorno
        if (blendingBack)
        {
            blendTimer += Time.deltaTime / returnBlendDuration;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, blendTimer);

            if (blendTimer >= 1f)
            {
                blendingBack = false;
                transform.rotation = endRotation;
            }
        }

        // Inicia el roll
        if (Input.GetKeyDown(KeyCode.E))
            StartRoll();
    }

    void StartRoll()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Dirección basada en cámara, pero sin rotar el personaje
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        if (moveDir.magnitude == 0)
            moveDir = transform.forward;

        rollDirection = moveDir;

        // Guardamos rotación inicial y final (para suavizar después)
        startRotation = transform.rotation;
        endRotation = Quaternion.LookRotation(transform.forward); // no rota, mantiene forward

        // Apagar máscara superior
        int upperBodyLayer = animator.GetLayerIndex("UpperBody");
        if (upperBodyLayer >= 0)
            animator.SetLayerWeight(upperBodyLayer, 0);

        animator.ResetTrigger("Roll");
        animator.SetTrigger("Roll");

        isRolling = true;
        rollTimer = rollDuration + rollCooldown;
    }

    void EndRoll()
    {
        isRolling = false;
        restoringMask = true;
        upperBodyWeight = 0f;

        // Suavizar retorno al idle (blend)
        blendingBack = true;
        blendTimer = 0f;

        // Blend suave hacia locomotion
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("RifleLocomotion"))
        {
            animator.CrossFadeInFixedTime("RifleLocomotion", returnBlendDuration);
        }

        animator.ResetTrigger("Roll");
    }
}
