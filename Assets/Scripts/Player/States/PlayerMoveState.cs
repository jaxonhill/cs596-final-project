public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerStateMachine currentContext) : base(currentContext)
    {
    }

    public override void EnterState()
    {
        Ctx.Animator.CrossFade(Ctx.MoveAnimationStateName, Ctx.AnimationCrossFadeDuration, 0);
    }

    public override void UpdateState()
    {
        Ctx.ApplyLocomotion();
    }

    public override void ExitState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.JumpPressed && Ctx.IsGrounded)
        {
            Ctx.SwitchState(Ctx.JumpState);
            return;
        }

        if (!Ctx.IsGrounded)
        {
            Ctx.SwitchState(Ctx.FallState);
            return;
        }

        if (!Ctx.HasMoveInput)
        {
            Ctx.SwitchState(Ctx.IdleState);
        }
    }
}
