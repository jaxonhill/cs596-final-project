public class PlayerRollState : PlayerBaseState
{
    public PlayerRollState(PlayerStateMachine currentContext) : base(currentContext)
    {
    }

    public override void EnterState()
    {
        PlayerAnimation rollAnimation = player.PlayerMotor.GetClosestRollAnimation(player.PlayerInput.MoveInput);
        player.PlayerMotor.BeginRoll(player.PlayerInput.MoveInput);
        player.PlayerAnimator.Play(rollAnimation);
    }

    public override void UpdateState()
    {
        player.PlayerMotor.ApplyRollMovement();
    }

    public override void ExitState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (!player.PlayerMotor.IsRollFinished)
        {
            return;
        }

        if (!player.PlayerMotor.IsGrounded)
        {
            player.SwitchState(player.FallState);
            return;
        }

        player.SwitchState(player.PlayerInput.IsTryingToMove ? player.MoveState : player.IdleState);
    }
}
