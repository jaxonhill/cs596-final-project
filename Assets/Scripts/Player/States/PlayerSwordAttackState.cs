namespace Player.States
{
    public class PlayerSwordAttackState : PlayerBaseState
    {
        public PlayerSwordAttackState(PlayerStateMachine currentContext) : base(currentContext)
        {
        }

        public override void EnterState()
        {
            player.PlayerMotor.StopHorizontalMovement();
            player.PlayerCombat.BeginSwordAttack();
            player.PlayerAnimator.Play(PlayerAnimation.SWORD_ATTACK);
            player.PlayerAudio?.PlaySwordSwing();
        }

        public override void UpdateState()
        {
            player.PlayerCombat.UpdateSwordAttack();
        }

        public override void ExitState()
        {
        }

        public override void CheckSwitchStates()
        {
            if (!player.PlayerCombat.IsSwordAttackFinished)
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
}
