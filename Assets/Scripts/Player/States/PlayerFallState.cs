public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine currentContext) : base(currentContext)
    {
    }

    public override void EnterState()
    {
        Ctx.Animator.CrossFade(Ctx.FallAnimationStateName, Ctx.AnimationCrossFadeDuration, 0);
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
        if (Ctx.IsGrounded)
        {
            Ctx.SwitchState(Ctx.HasMoveInput ? Ctx.MoveState : Ctx.IdleState);
        }
    }
}
