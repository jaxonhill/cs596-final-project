using UnityEngine;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode rollKey = KeyCode.LeftShift;
    [SerializeField] private float movementInputThreshold = 0.001f;

    private float movementThresholdSqr;

    public Vector2 MoveInput { get; private set; }
    public float TurnInput { get; private set; }
    public bool IsJumpPressed { get; private set; }
    public bool IsRollPressed { get; private set; }
    public bool IsSwordAttackPressed { get; private set; }
    public bool IsTryingToMove => MoveInput.sqrMagnitude > movementThresholdSqr;

    private void Awake()
    {
        movementThresholdSqr = movementInputThreshold * movementInputThreshold;
    }

    public void ReadInput()
    {
        TurnInput = Input.GetAxis("Mouse X");
        MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        IsJumpPressed = Input.GetKeyDown(jumpKey);
        IsRollPressed = Input.GetKeyDown(rollKey);
        IsSwordAttackPressed = Input.GetMouseButtonDown(0);
    }
}
