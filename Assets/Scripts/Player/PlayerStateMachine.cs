using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerStateMachine : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;

    [Header("Cursor")]
    [SerializeField] private bool lockCursorOnStart = true;
    [SerializeField] private KeyCode unlockCursorKey = KeyCode.Escape;

    [Header("Input")]
    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode rollKey = KeyCode.LeftShift;
    [SerializeField] private string turnAxis = "Mouse X";

    [Header("Movement")]
    [SerializeField] private float turnSensitivity = 180f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float accelerationRate = 32f;
    [SerializeField] private float decelerationRate = 40f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float groundedVerticalVelocity = -2f;
    [SerializeField] private float movementInputThreshold = 0.001f;

    [Header("Roll")]
    [SerializeField] private float rollSpeed = 8f;
    [SerializeField] private float rollDuration = 0.8f;

    [Header("Sword Attack")]
    [SerializeField] private GameObject swordAttackHitboxPrefab;
    [SerializeField] private int swordAttackDamage = 1;
    [SerializeField] private LayerMask swordAttackTargetLayers = ~0;
    [SerializeField] private float swordAttackDuration = 0.55f;
    [SerializeField] private float swordAttackHitboxDelay = 0.15f;
    [SerializeField] private float swordAttackLungeSpeed = 3.5f;
    [SerializeField] private float swordAttackLungeDuration = 0.12f;
    [SerializeField] private Vector3 swordAttackHitboxLocalOffset = new Vector3(0f, 1f, 1f);

    [Header("Animation")]
    [SerializeField] private string idleAnimationStateName = "Idle";
    [SerializeField] private string moveAnimationStateName = "Move";
    [SerializeField] private string jumpAnimationStateName = "Jump";
    [SerializeField] private string fallAnimationStateName = "Fall";
    [SerializeField] private string swordAttackAnimationStateName = "SwordAttack";
    [SerializeField] private string rollForwardAnimationStateName = "RollForward";
    [SerializeField] private string rollBackwardAnimationStateName = "RollBackward";
    [SerializeField] private string rollLeftAnimationStateName = "RollLeft";
    [SerializeField] private string rollRightAnimationStateName = "RollRight";
    [SerializeField] private float animationCrossFadeDuration = 0.1f;

    private float movementThresholdSqr;

    private PlayerBaseState currentState;
    private PlayerIdleState idleState;
    private PlayerMoveState moveState;
    private PlayerJumpState jumpState;
    private PlayerFallState fallState;
    private PlayerRollState rollState;
    private PlayerSwordAttackState swordAttackState;

    private Vector3 rollDirection;
    private string currentRollAnimationStateName;
    private float rollEndTime;
    private Vector3 swordAttackDirection;
    private float swordAttackEndTime;
    private float swordAttackHitboxSpawnTime;
    private float swordAttackLungeEndTime;
    private bool swordAttackHitboxSpawned;

    public PlayerBaseState CurrentState => currentState;
    public PlayerIdleState IdleState => idleState;
    public PlayerMoveState MoveState => moveState;
    public PlayerJumpState JumpState => jumpState;
    public PlayerFallState FallState => fallState;
    public PlayerRollState RollState => rollState;
    public PlayerSwordAttackState SwordAttackState => swordAttackState;

    public Vector2 MoveInput { get; private set; }
    public float TurnInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool RollPressed { get; private set; }
    public bool SwordAttackPressed { get; private set; }
    public Vector3 HorizontalVelocity { get; set; }
    public float VerticalVelocity { get; set; }

    public bool HasMoveInput => MoveInput.sqrMagnitude > movementThresholdSqr;
    public bool IsGrounded => characterController.isGrounded;
    public bool IsRollFinished => Time.time >= rollEndTime;
    public bool IsSwordAttackFinished => Time.time >= swordAttackEndTime;
    public float MovementThresholdSqr => movementThresholdSqr;
    public CharacterController CharacterController => characterController;
    public Animator Animator => animator;
    public float MoveSpeed => moveSpeed;
    public float AccelerationRate => accelerationRate;
    public float DecelerationRate => decelerationRate;
    public float RollSpeed => rollSpeed;
    public float Gravity => gravity;
    public float GroundedVerticalVelocity => groundedVerticalVelocity;
    public string IdleAnimationStateName => idleAnimationStateName;
    public string MoveAnimationStateName => moveAnimationStateName;
    public string JumpAnimationStateName => jumpAnimationStateName;
    public string FallAnimationStateName => fallAnimationStateName;
    public string SwordAttackAnimationStateName => swordAttackAnimationStateName;
    public string CurrentRollAnimationStateName => currentRollAnimationStateName;
    public float AnimationCrossFadeDuration => animationCrossFadeDuration;

    private void Awake()
    {
        movementThresholdSqr = movementInputThreshold * movementInputThreshold;
        idleState = new PlayerIdleState(this);
        moveState = new PlayerMoveState(this);
        jumpState = new PlayerJumpState(this);
        fallState = new PlayerFallState(this);
        rollState = new PlayerRollState(this);
        swordAttackState = new PlayerSwordAttackState(this);
    }

    private void Start()
    {
        if (lockCursorOnStart)
        {
            LockCursor();
        }

        SwitchState(IsGrounded ? idleState : fallState);
    }

    private void Update()
    {
        UpdateCursorLock();
        ReadInput();
        if (currentState != rollState)
        {
            TurnPlayer();
        }

        currentState.UpdateState();
        currentState.CheckSwitchStates();
    }

    public void SwitchState(PlayerBaseState newState)
    {
        if (newState == null || newState == currentState)
        {
            return;
        }

        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    private void ReadInput()
    {
        MoveInput = new Vector2(Input.GetAxisRaw(horizontalAxis), Input.GetAxisRaw(verticalAxis));
        JumpPressed = Input.GetKeyDown(jumpKey);
        RollPressed = Input.GetKeyDown(rollKey);
        SwordAttackPressed = Input.GetMouseButtonDown(0);
        TurnInput = Input.GetAxis(turnAxis);
    }

    public void ApplyLocomotion()
    {
        Vector2 clampedInput = Vector2.ClampMagnitude(MoveInput, 1f);
        Vector3 moveDirection = (transform.right * clampedInput.x + transform.forward * clampedInput.y).normalized;
        Vector3 targetVelocity = moveDirection * moveSpeed;

        float blendRate = clampedInput.sqrMagnitude > movementThresholdSqr ? accelerationRate : decelerationRate;
        HorizontalVelocity = Vector3.MoveTowards(HorizontalVelocity, targetVelocity, blendRate * Time.deltaTime);

        MoveWithGravity(HorizontalVelocity);
    }

    public void BeginRoll()
    {
        rollDirection = GetDesiredRollDirection();
        currentRollAnimationStateName = GetClosestRollAnimationStateName(rollDirection);
        rollEndTime = Time.time + rollDuration;
        HorizontalVelocity = Vector3.zero;

        if (IsGrounded && VerticalVelocity < 0f)
        {
            VerticalVelocity = groundedVerticalVelocity;
        }
    }

    public void ApplyRollMovement()
    {
        MoveWithGravity(rollDirection * rollSpeed);
    }

    public void BeginSwordAttack()
    {
        swordAttackDirection = GetDesiredActionDirection();
        swordAttackEndTime = Time.time + swordAttackDuration;
        swordAttackHitboxSpawnTime = Time.time + swordAttackHitboxDelay;
        swordAttackLungeEndTime = Time.time + swordAttackLungeDuration;
        swordAttackHitboxSpawned = false;
        HorizontalVelocity = Vector3.zero;

        if (IsGrounded && VerticalVelocity < 0f)
        {
            VerticalVelocity = groundedVerticalVelocity;
        }
    }

    public void UpdateSwordAttack()
    {
        if (!swordAttackHitboxSpawned && Time.time >= swordAttackHitboxSpawnTime)
        {
            SpawnSwordAttackHitbox();
        }

        Vector3 attackVelocity = Time.time < swordAttackLungeEndTime ? swordAttackDirection * swordAttackLungeSpeed : Vector3.zero;
        MoveWithGravity(attackVelocity);
    }

    public void Jump()
    {
        VerticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void MoveWithGravity(Vector3 horizontalMovement)
    {
        HorizontalVelocity = horizontalMovement;

        if (IsGrounded && VerticalVelocity < 0f)
        {
            VerticalVelocity = groundedVerticalVelocity;
        }

        VerticalVelocity += gravity * Time.deltaTime;

        Vector3 totalVelocity = HorizontalVelocity + Vector3.up * VerticalVelocity;
        characterController.Move(totalVelocity * Time.deltaTime);
    }

    private void SpawnSwordAttackHitbox()
    {
        swordAttackHitboxSpawned = true;

        if (swordAttackHitboxPrefab == null)
        {
            Debug.LogWarning("Sword attack hitbox prefab is not assigned.", this);
            return;
        }

        Vector3 spawnPosition = transform.TransformPoint(swordAttackHitboxLocalOffset);
        GameObject spawnedHitbox = Instantiate(swordAttackHitboxPrefab, spawnPosition, transform.rotation);
        SwordAttackHitbox hitbox = spawnedHitbox.GetComponent<SwordAttackHitbox>();

        if (hitbox != null)
        {
            hitbox.Initialize(gameObject, swordAttackDamage, swordAttackTargetLayers);
            return;
        }

        Debug.LogWarning("Sword attack hitbox prefab is missing a SwordAttackHitbox component.", spawnedHitbox);
    }

    private Vector3 GetDesiredActionDirection()
    {
        if (!HasMoveInput)
        {
            return transform.forward;
        }

        Vector2 clampedInput = Vector2.ClampMagnitude(MoveInput, 1f);
        return (transform.right * clampedInput.x + transform.forward * clampedInput.y).normalized;
    }

    private Vector3 GetDesiredRollDirection()
    {
        return GetDesiredActionDirection();
    }

    private string GetClosestRollAnimationStateName(Vector3 desiredDirection)
    {
        float forwardDot = Vector3.Dot(desiredDirection, transform.forward);
        float backwardDot = Vector3.Dot(desiredDirection, -transform.forward);
        float leftDot = Vector3.Dot(desiredDirection, -transform.right);
        float rightDot = Vector3.Dot(desiredDirection, transform.right);

        string closestAnimation = rollForwardAnimationStateName;
        float highestDot = forwardDot;

        if (backwardDot > highestDot)
        {
            highestDot = backwardDot;
            closestAnimation = rollBackwardAnimationStateName;
        }

        if (leftDot > highestDot)
        {
            highestDot = leftDot;
            closestAnimation = rollLeftAnimationStateName;
        }

        if (rightDot > highestDot)
        {
            closestAnimation = rollRightAnimationStateName;
        }

        return closestAnimation;
    }

    private void TurnPlayer()
    {
        transform.Rotate(
            Vector3.up,
            TurnInput * turnSensitivity * Time.deltaTime,
            Space.World
        );
    }

    private void UpdateCursorLock()
    {
        if (Input.GetKeyDown(unlockCursorKey))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        if (lockCursorOnStart && Input.GetMouseButtonDown(0))
        {
            LockCursor();
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
