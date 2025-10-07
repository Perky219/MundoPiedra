using UnityEngine;

public class Roll : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;

    [Header("Roll Settings")]
    public float rollCooldown = 0.2f;
    public float maskBlendSpeed = 6f;
    public float returnBlendDuration = 0.25f; // transición suave al idle

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

        // 🔸 Blend suave para restaurar la máscara del cuerpo superior
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

        // 🔸 Durante el roll (movimiento controlado por Root Motion)
        if (isRolling)
        {
            rollTimer -= Time.deltaTime;

            if (rollTimer <= 0f)
                EndRoll();

            return;
        }

        // 🔸 Blend suave de rotación de vuelta al idle
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

        // 🔸 Inicia el roll
        if (Input.GetKeyDown(KeyCode.E))
            StartRoll();
    }

    void StartRoll()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Dirección basada en cámara
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;
        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        if (moveDir.magnitude == 0)
            moveDir = transform.forward;

        rollDirection = moveDir;

        // Guardar rotaciones
        startRotation = transform.rotation;
        endRotation = Quaternion.LookRotation(transform.forward);

        // Apagar máscara superior
        int upperBodyLayer = animator.GetLayerIndex("UpperBody");
        if (upperBodyLayer >= 0)
            animator.SetLayerWeight(upperBodyLayer, 0);

        animator.ResetTrigger("Roll");
        animator.SetTrigger("Roll");

        // 🔸 Activar root motion solo durante el roll
        animator.applyRootMotion = true;

        // Obtener duración del clip activo
        float clipLength = 1f;
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
            clipLength = clipInfo[0].clip.length;

        rollTimer = clipLength + rollCooldown;
        isRolling = true;
    }

    void EndRoll()
    {
        isRolling = false;
        restoringMask = true;
        upperBodyWeight = 0f;
        blendingBack = true;
        blendTimer = 0f;

        // 🔸 Desactivar root motion al terminar
        animator.applyRootMotion = false;

        // 🔸 Transición suave a locomotion
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("RifleLocomotion"))
        {
            animator.CrossFadeInFixedTime("RifleLocomotion", returnBlendDuration);
        }

        animator.ResetTrigger("Roll");
    }
}
