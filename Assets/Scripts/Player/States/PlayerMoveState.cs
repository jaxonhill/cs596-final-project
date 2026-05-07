using UnityEngine;

namespace Player.States
{
    public class PlayerMoveState : PlayerBaseState
    {
        private const float FootstepIntervalSeconds = 0.35f;
        private float nextFootstepTime;

        public PlayerMoveState(PlayerStateMachine currentContext) : base(currentContext)
        {
        }

        public override void EnterState()
        {
            player.PlayerAnimator.Play(PlayerAnimation.MOVE_F);
            nextFootstepTime = Time.time;
        }

        public override void UpdateState()
        {
            player.PlayerMotor.ApplyLocomotion(player.PlayerInput.MoveInput);

            if (player.PlayerMotor.IsGrounded && Time.time >= nextFootstepTime)
            {
                player.PlayerAudio?.OnFootstep();
                nextFootstepTime = Time.time + FootstepIntervalSeconds;
            }
        }

        public override void ExitState()
        {
        }

        public override void CheckSwitchStates()
        {
            if (player.PlayerInput.IsSwordAttackPressed && player.PlayerMotor.IsGrounded)
            {
                player.SwitchState(player.SwordAttackState);
                return;
            }

            if (player.PlayerInput.IsRollPressed && player.PlayerMotor.IsGrounded)
            {
                player.SwitchState(player.RollState);
                return;
            }

            if (player.PlayerInput.IsJumpPressed && player.PlayerMotor.IsGrounded)
            {
                player.SwitchState(player.JumpState);
                return;
            }

            if (!player.PlayerMotor.IsGrounded)
            {
                player.SwitchState(player.FallState);
                return;
            }

            if (!player.PlayerInput.IsTryingToMove)
            {
                player.SwitchState(player.IdleState);
            }
        }
    }
}
