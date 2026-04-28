using UnityEngine;

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
        Vector2 clampedInput = Vector2.ClampMagnitude(Ctx.MoveInput, 1f);
        Vector3 moveDirection = (Ctx.transform.right * clampedInput.x + Ctx.transform.forward * clampedInput.y).normalized;
        Vector3 targetVelocity = moveDirection * Ctx.MoveSpeed;

        float blendRate = clampedInput.sqrMagnitude > Ctx.MovementThresholdSqr ? Ctx.AccelerationRate : Ctx.DecelerationRate;
        Ctx.HorizontalVelocity = Vector3.MoveTowards(Ctx.HorizontalVelocity, targetVelocity, blendRate * Time.deltaTime);

        Ctx.CharacterController.Move(Ctx.HorizontalVelocity * Time.deltaTime);
    }

    public override void ExitState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.HasMoveInput)
        {
            Ctx.SwitchState(Ctx.IdleState);
        }
    }
}
