public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext) : base(currentContext)
    {
    }

    public override void EnterState()
    {
        Ctx.Jump();
        Ctx.Animator.CrossFade(Ctx.JumpAnimationStateName, Ctx.AnimationCrossFadeDuration, 0);
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
        if (Ctx.VerticalVelocity <= 0f)
        {
            Ctx.SwitchState(Ctx.FallState);
        }
    }
}
