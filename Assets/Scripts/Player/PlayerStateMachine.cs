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
    [SerializeField] private string turnAxis = "Mouse X";
    [SerializeField] private float turnSensitivity = 180f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float accelerationRate = 32f;
    [SerializeField] private float decelerationRate = 40f;
    [SerializeField] private float movementInputThreshold = 0.001f;

    [Header("Animation")]
    [SerializeField] private string idleAnimationStateName = "Idle";
    [SerializeField] private string moveAnimationStateName = "Move";
    [SerializeField] private float animationCrossFadeDuration = 0.1f;

    private float movementThresholdSqr;

    private PlayerBaseState currentState;
    private PlayerIdleState idleState;
    private PlayerMoveState moveState;

    public PlayerBaseState CurrentState => currentState;
    public PlayerIdleState IdleState => idleState;
    public PlayerMoveState MoveState => moveState;

    public Vector2 MoveInput { get; private set; }
    public float TurnInput { get; private set; }
    public Vector3 HorizontalVelocity { get; set; }

    public bool HasMoveInput => MoveInput.sqrMagnitude > movementThresholdSqr;
    public float MovementThresholdSqr => movementThresholdSqr;
    public CharacterController CharacterController => characterController;
    public Animator Animator => animator;
    public float MoveSpeed => moveSpeed;
    public float AccelerationRate => accelerationRate;
    public float DecelerationRate => decelerationRate;
    public string IdleAnimationStateName => idleAnimationStateName;
    public string MoveAnimationStateName => moveAnimationStateName;
    public float AnimationCrossFadeDuration => animationCrossFadeDuration;

    private void Awake()
    {
        movementThresholdSqr = movementInputThreshold * movementInputThreshold;
        idleState = new PlayerIdleState(this);
        moveState = new PlayerMoveState(this);
    }

    private void Start()
    {
        if (lockCursorOnStart)
        {
            LockCursor();
        }

        SwitchState(idleState);
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
        MoveInput = new Vector2(Input.GetAxisRaw(horizontalAxis), Input.GetAxisRaw(verticalAxis));
        TurnInput = Input.GetAxis(turnAxis);
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
