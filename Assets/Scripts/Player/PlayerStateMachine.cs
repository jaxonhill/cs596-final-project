using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(Damageable))]
public class PlayerStateMachine : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private PlayerInputReader playerInput;
    [SerializeField] private PlayerMotor playerMotor;
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private Damageable playerDamageable;

    private PlayerBaseState currentState;
    private PlayerIdleState idleState;
    private PlayerMoveState moveState;
    private PlayerJumpState jumpState;
    private PlayerFallState fallState;
    private PlayerRollState rollState;
    private PlayerSwordAttackState swordAttackState;

    public PlayerInputReader PlayerInput => playerInput;
    public PlayerMotor PlayerMotor => playerMotor;
    public PlayerAnimator PlayerAnimator => playerAnimator;
    public PlayerCombat PlayerCombat => playerCombat;
    public Damageable PlayerDamageable => playerDamageable;

    public PlayerBaseState CurrentState => currentState;
    public PlayerIdleState IdleState => idleState;
    public PlayerMoveState MoveState => moveState;
    public PlayerJumpState JumpState => jumpState;
    public PlayerFallState FallState => fallState;
    public PlayerRollState RollState => rollState;
    public PlayerSwordAttackState SwordAttackState => swordAttackState;

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

    private void Awake()
    {
        LogRequiredReferences();

        idleState = new PlayerIdleState(this);
        moveState = new PlayerMoveState(this);
        jumpState = new PlayerJumpState(this);
        fallState = new PlayerFallState(this);
        rollState = new PlayerRollState(this);
        swordAttackState = new PlayerSwordAttackState(this);
    }

    private void Start()
    {
        if (!HasRequiredReferences())
        {
            Debug.LogError("PlayerStateMachine cannot start because one or more required references are missing.", this);
            enabled = false;
            return;
        }

        SwitchState(idleState);
    }

    private void Update()
    {
        PlayerInput.ReadInput();

        if (currentState != rollState)
        {
            PlayerMotor.Turn(PlayerInput.TurnInput);
        }

        currentState.UpdateState();
        currentState.CheckSwitchStates();
    }

    private bool HasRequiredReferences()
    {
        return playerInput != null
            && playerMotor != null
            && playerAnimator != null
            && playerCombat != null
            && playerDamageable != null;
    }

    private void LogRequiredReferences()
    {
        LogRequiredReference(playerInput, nameof(playerInput), typeof(PlayerInputReader).Name);
        LogRequiredReference(playerMotor, nameof(playerMotor), typeof(PlayerMotor).Name);
        LogRequiredReference(playerAnimator, nameof(playerAnimator), typeof(PlayerAnimator).Name);
        LogRequiredReference(playerCombat, nameof(playerCombat), typeof(PlayerCombat).Name);
        LogRequiredReference(playerDamageable, nameof(playerDamageable), typeof(Damageable).Name);
    }

    private void LogRequiredReference(Object reference, string fieldName, string componentName)
    {
        if (reference == null)
        {
            Debug.LogError($"PlayerStateMachine requires {componentName} assigned to '{fieldName}' on {name}.", this);
            return;
        }

        Debug.Log($"PlayerStateMachine found required {componentName} reference on {name}.", this);
    }
}
