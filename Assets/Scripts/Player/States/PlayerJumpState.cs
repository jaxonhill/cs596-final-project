public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext) : base(currentContext)
    {
    }

    public override void EnterState()
    {
        player.PlayerMotor.Jump();
        player.PlayerAnimator.Play(PlayerAnimation.JUMP);
    }

    public override void UpdateState()
    {
        player.PlayerMotor.ApplyLocomotion(player.PlayerInput.MoveInput);
    }

    public override void ExitState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (player.PlayerMotor.VerticalVelocity <= 0f)
        {
            player.SwitchState(player.FallState);
        }
    }
}
