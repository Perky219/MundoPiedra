using UnityEngine;

public class Roll : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;

    [Header("Roll Settings")]
    public float rollSpeed = 6f;
    public float rollMovePercent = 0.7f;
    public float rollCooldown = 0.2f;
    public float maskBlendSpeed = 6f;
    public float returnBlendDuration = 0.25f;
    public float maxRollFailsafe = 3.0f;

    [Header("Animator States")]
    public string rollStateName = "Roll";
    public string locomotionStateName = "RifleLocomotion";

    private bool isRolling = false;
    private bool restoringMask = false;
    private bool blendingBack = false;
    private bool inCooldown = false;

    private Vector3 rollDirection;
    private float upperBodyWeight = 1f;
    private Quaternion startRotation;
    private Quaternion endRotation;
    private float blendTimer = 0f;
    private float failsafeTimer = 0f;

    private int upperBodyLayerIndex = -1;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        upperBodyLayerIndex = animator.GetLayerIndex("UpperBody");
    }

    void Update()
    {
        if (restoringMask && upperBodyLayerIndex >= 0)
        {
            upperBodyWeight = Mathf.Lerp(upperBodyWeight, 1f, Time.deltaTime * maskBlendSpeed);
            animator.SetLayerWeight(upperBodyLayerIndex, upperBodyWeight);

            if (upperBodyWeight > 0.98f)
            {
                restoringMask = false;
                animator.SetLayerWeight(upperBodyLayerIndex, 1f);
            }
        }

        if (isRolling)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            bool inRollState = state.IsName(rollStateName);

            if (inRollState && state.normalizedTime < rollMovePercent)
                controller.Move(rollDirection * rollSpeed * Time.deltaTime);

            failsafeTimer += Time.deltaTime;
            bool finished = inRollState && state.normalizedTime >= 0.99f && !animator.IsInTransition(0);
            if (finished || failsafeTimer >= maxRollFailsafe)
                EndRoll();

            return;
        }

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

        if (!inCooldown && !isRolling && Input.GetKeyDown(KeyCode.Space))
            StartRoll();
    }

    void StartRoll()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camF = Camera.main.transform.forward; camF.y = 0;
        Vector3 camR = Camera.main.transform.right;   camR.y = 0;
        Vector3 moveDir = (camF * v + camR * h).normalized;
        if (moveDir.sqrMagnitude < 0.0001f) moveDir = transform.forward;

        rollDirection = moveDir.normalized;

        startRotation = transform.rotation;
        endRotation = Quaternion.LookRotation(rollDirection);
        transform.rotation = endRotation;

        if (upperBodyLayerIndex >= 0)
        {
            upperBodyWeight = 0f;
            animator.SetLayerWeight(upperBodyLayerIndex, 0f);
        }

        animator.ResetTrigger("Roll");
        animator.SetTrigger("Roll");

        isRolling = true;
        blendingBack = false;
        restoringMask = false;
        inCooldown = true;
        failsafeTimer = 0f;
    }

    void EndRoll()
    {
        isRolling = false;
        restoringMask = true;
        blendingBack = true;
        blendTimer = 0f;

        animator.ResetTrigger("Roll");

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if (!state.IsName(locomotionStateName))
        {
            animator.CrossFadeInFixedTime(locomotionStateName, returnBlendDuration);
        }

        Invoke(nameof(ReleaseCooldown), rollCooldown);
    }

    void ReleaseCooldown()
    {
        inCooldown = false;
        if (upperBodyLayerIndex >= 0) animator.SetLayerWeight(upperBodyLayerIndex, 1f);
        upperBodyWeight = 1f;
    }
}
