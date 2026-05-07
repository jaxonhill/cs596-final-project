public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext) : base(currentContext)
    {
    }

    public override void EnterState()
    {
        player.PlayerMotor.StopHorizontalMovement();
        player.PlayerAnimator.Play(PlayerAnimation.IDLE);
    }

    public override void UpdateState()
    {
    }

    public override void ExitState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (player.PlayerInput.IsSwordAttackPressed && player.PlayerMotor.IsGrounded)
        {
            player.SwitchState(player.SwordAttackState);
            return;
        }

        if (player.PlayerInput.IsRollPressed && player.PlayerMotor.IsGrounded)
        {
            player.SwitchState(player.RollState);
            return;
        }

        if (player.PlayerInput.IsJumpPressed && player.PlayerMotor.IsGrounded)
        {
            player.SwitchState(player.JumpState);
            return;
        }

        if (!player.PlayerMotor.IsGrounded)
        {
            player.SwitchState(player.FallState);
            return;
        }

        if (player.PlayerInput.IsTryingToMove)
        {
            player.SwitchState(player.MoveState);
        }
    }
}
