public class PlayerSwordAttackState : PlayerBaseState
{
    public PlayerSwordAttackState(PlayerStateMachine currentContext) : base(currentContext)
    {
    }

    public override void EnterState()
    {
        Ctx.BeginSwordAttack();
        Ctx.Animator.CrossFade(Ctx.SwordAttackAnimationStateName, Ctx.AnimationCrossFadeDuration, 0);
    }

    public override void UpdateState()
    {
        Ctx.UpdateSwordAttack();
    }

    public override void ExitState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsSwordAttackFinished)
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
