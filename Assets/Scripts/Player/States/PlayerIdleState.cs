using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext) : base(currentContext)
    {
    }

    public override void EnterState()
    {
        Ctx.HorizontalVelocity = Vector3.zero;
        Ctx.Animator.CrossFade(Ctx.IdleAnimationStateName, Ctx.AnimationCrossFadeDuration, 0);
    }

    public override void UpdateState()
    {
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

        if (Ctx.HasMoveInput)
        {
            Ctx.SwitchState(Ctx.MoveState);
        }
    }
}
