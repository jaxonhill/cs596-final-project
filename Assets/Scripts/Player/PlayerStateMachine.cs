using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(Damageable))]
public class PlayerStateMachine : MonoBehaviour
{
    // Player Components
    public PlayerInputReader PlayerInput { get; private set; }
    public PlayerMotor PlayerMotor { get; private set; }
    public PlayerAnimator PlayerAnimator { get; private set; }
    public PlayerCombat PlayerCombat { get; private set; }
    public Damageable PlayerDamageable { get; private set; }

    // States
    public PlayerBaseState CurrentState { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerFallState FallState { get; private set; }
    public PlayerRollState RollState { get; private set; }
    public PlayerSwordAttackState SwordAttackState { get; private set; }

    private void Awake()
    {
        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        JumpState = new PlayerJumpState(this);
        FallState = new PlayerFallState(this);
        RollState = new PlayerRollState(this);
        SwordAttackState = new PlayerSwordAttackState(this);
    }

    private void Start()
    {
        SwitchState(IdleState);
    }

    private void Update()
    {
        PlayerInput.ReadInput();

        if (CurrentState != RollState)
        {
            PlayerMotor.Turn(PlayerInput.TurnInput);
        }

        CurrentState.UpdateState();
        CurrentState.CheckSwitchStates();
    }

    public void SwitchState(PlayerBaseState newState)
    {
        if (newState == CurrentState)
        {
            return;
        }

        CurrentState.ExitState();
        CurrentState = newState;
        CurrentState.EnterState();
    }
}
