using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class TopDownPlayerMotor : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 25f;

    [Header("Character Controller")]
    [SerializeField] private float controllerHeight = 2f;
    [SerializeField] private float controllerRadius = 0.5f;
    [SerializeField] private Vector3 controllerCenter = Vector3.zero;
    [SerializeField] private float stepOffset = 0.3f;
    [SerializeField] private float slopeLimit = 45f;
    [SerializeField] private float skinWidth = 0.08f;
    [SerializeField] private float minMoveDistance = 0f;

    [Header("Debug")]
    [SerializeField] private bool verboseLogs = true;

    private CharacterController characterController;
    private Vector3 currentVelocity;
    private bool wasMoving;
    private bool wasBlockedByWall;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            if (verboseLogs)
            {
                Debug.Log("[TopDownPlayerMotor] Added missing CharacterController automatically.", this);
            }
        }

        ApplyControllerSettings();

        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider != null && capsuleCollider.enabled)
        {
            capsuleCollider.enabled = false;
            if (verboseLogs)
            {
                Debug.Log("[TopDownPlayerMotor] Disabled CapsuleCollider to avoid overlapping player colliders.", this);
            }
        }
    }

    private void Start()
    {
        if (!verboseLogs)
        {
            return;
        }

        Debug.Log(
            $"[TopDownPlayerMotor] Ready. moveSpeed={moveSpeed:F2}, accel={acceleration:F2}, decel={deceleration:F2}, inputBackend={GetInputBackendSummary()}.",
            this);
    }

    private void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput.Normalize();
        }

        Vector3 targetVelocity = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        float blendRate = moveInput.sqrMagnitude > 0.001f ? acceleration : deceleration;
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, blendRate * Time.deltaTime);

        CollisionFlags collisionFlags = characterController.Move(currentVelocity * Time.deltaTime);

        bool isMoving = currentVelocity.sqrMagnitude > 0.01f;
        if (verboseLogs && isMoving != wasMoving)
        {
            Debug.Log(isMoving
                ? "[TopDownPlayerMotor] Movement started."
                : "[TopDownPlayerMotor] Movement stopped.", this);
        }

        bool isBlockedByWall = (collisionFlags & CollisionFlags.Sides) != 0;
        if (verboseLogs && isBlockedByWall != wasBlockedByWall)
        {
            Debug.Log(isBlockedByWall
                ? "[TopDownPlayerMotor] Side collision detected."
                : "[TopDownPlayerMotor] Side collision cleared.", this);
        }

        wasMoving = isMoving;
        wasBlockedByWall = isBlockedByWall;
    }

    private void ApplyControllerSettings()
    {
        characterController.height = Mathf.Max(0.01f, controllerHeight);
        characterController.radius = Mathf.Max(0.01f, controllerRadius);
        characterController.center = controllerCenter;
        characterController.stepOffset = Mathf.Max(0f, stepOffset);
        characterController.slopeLimit = Mathf.Clamp(slopeLimit, 1f, 89f);
        characterController.skinWidth = Mathf.Max(0.0001f, skinWidth);
        characterController.minMoveDistance = Mathf.Max(0f, minMoveDistance);
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        acceleration = Mathf.Max(0.01f, acceleration);
        deceleration = Mathf.Max(0.01f, deceleration);
        controllerHeight = Mathf.Max(0.01f, controllerHeight);
        controllerRadius = Mathf.Max(0.01f, controllerRadius);
        stepOffset = Mathf.Max(0f, stepOffset);
        slopeLimit = Mathf.Clamp(slopeLimit, 1f, 89f);
        skinWidth = Mathf.Max(0.0001f, skinWidth);
        minMoveDistance = Mathf.Max(0f, minMoveDistance);

        if (characterController != null)
        {
            ApplyControllerSettings();
        }
    }

    private static string GetInputBackendSummary()
    {
#if ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
        return "Both (Input System + Legacy Input Manager)";
#elif ENABLE_INPUT_SYSTEM
        return "Input System Only";
#elif ENABLE_LEGACY_INPUT_MANAGER
        return "Legacy Input Manager Only";
#else
        return "Unknown";
#endif
    }
}
