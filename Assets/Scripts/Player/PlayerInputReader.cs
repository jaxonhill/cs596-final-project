using UnityEngine;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private float movementInputThreshold = 0.001f;

    private float movementThresholdSqr;
    private bool queuedJumpInput;
    private bool queuedRollInput;
    private bool queuedSwordAttackInput;

    public Vector2 MoveInput { get; private set; }
    public float TurnInput { get; private set; }
    public bool IsJumpPressed { get; private set; }
    public bool IsRollPressed { get; private set; }
    public bool IsSwordAttackPressed { get; private set; }
    public bool IsTryingToMove => MoveInput.sqrMagnitude > movementThresholdSqr;

    private void Awake()
    {
        movementThresholdSqr = movementInputThreshold * movementInputThreshold;
        Debug.Log($"PlayerInputReader initialized on {name}.", this);
    }

    private void OnDisable()
    {
        ClearMoveInput();
        queuedJumpInput = false;
        queuedRollInput = false;
        queuedSwordAttackInput = false;
        IsJumpPressed = false;
        IsRollPressed = false;
        IsSwordAttackPressed = false;
        TurnInput = 0f;
    }

    public void ReadInput()
    {
        TurnInput = 0f;

        IsJumpPressed = queuedJumpInput;
        IsRollPressed = queuedRollInput;
        IsSwordAttackPressed = queuedSwordAttackInput;

        queuedJumpInput = false;
        queuedRollInput = false;
        queuedSwordAttackInput = false;

        LogConsumedActionInputs();
    }

    public void SetMoveInput(Vector2 moveInput)
    {
        Vector2 clampedInput = Vector2.ClampMagnitude(moveInput, 1f);
        MoveInput = clampedInput.sqrMagnitude > movementThresholdSqr ? clampedInput : Vector2.zero;
    }

    public void ClearMoveInput()
    {
        MoveInput = Vector2.zero;
    }

    public void QueueJumpInput()
    {
        queuedJumpInput = true;
        Debug.Log($"PlayerInputReader queued jump input on {name}.", this);
    }

    public void QueueRollInput()
    {
        queuedRollInput = true;
        Debug.Log($"PlayerInputReader queued roll input on {name}.", this);
    }

    public void QueueSwordAttackInput()
    {
        queuedSwordAttackInput = true;
        Debug.Log($"PlayerInputReader queued sword attack input on {name}.", this);
    }

    private void LogConsumedActionInputs()
    {
        if (IsJumpPressed)
        {
            Debug.Log($"PlayerInputReader consumed jump input on {name}.", this);
        }

        if (IsRollPressed)
        {
            Debug.Log($"PlayerInputReader consumed roll input on {name}.", this);
        }

        if (IsSwordAttackPressed)
        {
            Debug.Log($"PlayerInputReader consumed sword attack input on {name}.", this);
        }
    }
}
