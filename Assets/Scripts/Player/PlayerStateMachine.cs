using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerCombat))]
public class PlayerStateMachine : MonoBehaviour
{
    private PlayerBaseState currentState;
    private PlayerIdleState idleState;
    private PlayerMoveState moveState;
    private PlayerJumpState jumpState;
    private PlayerFallState fallState;
    private PlayerRollState rollState;
    private PlayerSwordAttackState swordAttackState;

    public PlayerInputReader PlayerInput { get; private set; }
    public PlayerMotor PlayerMotor { get; private set; }
    public PlayerAnimator PlayerAnimator { get; private set; }
    public PlayerCombat PlayerCombat { get; private set; }

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
        idleState = new PlayerIdleState(this);
        moveState = new PlayerMoveState(this);
        jumpState = new PlayerJumpState(this);
        fallState = new PlayerFallState(this);
        rollState = new PlayerRollState(this);
        swordAttackState = new PlayerSwordAttackState(this);
    }

    private void Start()
    {
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
}
