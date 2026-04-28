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

    [Header("Movement")]
    [SerializeField] private float turnSensitivity = 180f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float accelerationRate = 32f;
    [SerializeField] private float decelerationRate = 40f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float groundedVerticalVelocity = -2f;
    [SerializeField] private float movementInputThreshold = 0.001f;

    [Header("Animation")]
    [SerializeField] private string idleAnimationStateName = "Idle";
    [SerializeField] private string moveAnimationStateName = "Move";
    [SerializeField] private string jumpAnimationStateName = "Jump";
    [SerializeField] private string fallAnimationStateName = "Fall";
    [SerializeField] private float animationCrossFadeDuration = 0.1f;

    private float movementThresholdSqr;

    private PlayerBaseState currentState;
    private PlayerIdleState idleState;
    private PlayerMoveState moveState;
    private PlayerJumpState jumpState;
    private PlayerFallState fallState;

    public PlayerBaseState CurrentState => currentState;
    public PlayerIdleState IdleState => idleState;
    public PlayerMoveState MoveState => moveState;
    public PlayerJumpState JumpState => jumpState;
    public PlayerFallState FallState => fallState;

    public Vector2 MoveInput { get; private set; }
    public float TurnInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public Vector3 HorizontalVelocity { get; set; }
    public float VerticalVelocity { get; set; }

    public bool HasMoveInput => MoveInput.sqrMagnitude > movementThresholdSqr;
    public bool IsGrounded => characterController.isGrounded;
    public float MovementThresholdSqr => movementThresholdSqr;
    public CharacterController CharacterController => characterController;
    public Animator Animator => animator;
    public float MoveSpeed => moveSpeed;
    public float AccelerationRate => accelerationRate;
    public float DecelerationRate => decelerationRate;
    public float Gravity => gravity;
    public float GroundedVerticalVelocity => groundedVerticalVelocity;
    public string IdleAnimationStateName => idleAnimationStateName;
    public string MoveAnimationStateName => moveAnimationStateName;
    public string JumpAnimationStateName => jumpAnimationStateName;
    public string FallAnimationStateName => fallAnimationStateName;
    public float AnimationCrossFadeDuration => animationCrossFadeDuration;

    private void Awake()
    {
        movementThresholdSqr = movementInputThreshold * movementInputThreshold;
        idleState = new PlayerIdleState(this);
        moveState = new PlayerMoveState(this);
        jumpState = new PlayerJumpState(this);
        fallState = new PlayerFallState(this);
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
        TurnPlayer();

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
        MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        JumpPressed = Input.GetButtonDown("Jump");
        TurnInput = Input.GetAxis("Mouse X");
    }

    public void ApplyLocomotion()
    {
        Vector2 clampedInput = Vector2.ClampMagnitude(MoveInput, 1f);
        Vector3 moveDirection = (transform.right * clampedInput.x + transform.forward * clampedInput.y).normalized;
        Vector3 targetVelocity = moveDirection * moveSpeed;

        float blendRate = clampedInput.sqrMagnitude > movementThresholdSqr ? accelerationRate : decelerationRate;
        HorizontalVelocity = Vector3.MoveTowards(HorizontalVelocity, targetVelocity, blendRate * Time.deltaTime);

        if (IsGrounded && VerticalVelocity < 0f)
        {
            VerticalVelocity = groundedVerticalVelocity;
        }

        VerticalVelocity += gravity * Time.deltaTime;

        Vector3 totalVelocity = HorizontalVelocity + Vector3.up * VerticalVelocity;
        characterController.Move(totalVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        VerticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
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
