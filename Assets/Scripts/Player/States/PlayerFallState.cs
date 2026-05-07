namespace Player.States
{
    public class PlayerFallState : PlayerBaseState
    {
        public PlayerFallState(PlayerStateMachine currentContext) : base(currentContext)
        {
        }

        public override void EnterState()
        {
            player.PlayerAnimator.Play(PlayerAnimation.FALL);
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
            if (player.PlayerMotor.IsGrounded)
            {
                player.SwitchState(player.PlayerInput.IsTryingToMove ? player.MoveState : player.IdleState);
            }
        }
    }
}
