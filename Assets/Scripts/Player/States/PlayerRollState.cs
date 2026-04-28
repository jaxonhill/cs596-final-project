public class PlayerRollState : PlayerBaseState
{
    public PlayerRollState(PlayerStateMachine currentContext) : base(currentContext)
    {
    }

    public override void EnterState()
    {
        Ctx.BeginRoll();
        Ctx.Animator.CrossFade(Ctx.CurrentRollAnimationStateName, Ctx.AnimationCrossFadeDuration, 0);
    }

    public override void UpdateState()
    {
        Ctx.ApplyRollMovement();
    }

    public override void ExitState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsRollFinished)
        {
            return;
        }

        if (!Ctx.IsGrounded)
        {
            Ctx.SwitchState(Ctx.FallState);
            return;
        }

        Ctx.SwitchState(Ctx.HasMoveInput ? Ctx.MoveState : Ctx.IdleState);
    }
}
