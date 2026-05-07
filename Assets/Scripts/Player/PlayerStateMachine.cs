using Combat;
using Player;
using Player.States;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(PlayerAnimator))]
    [RequireComponent(typeof(PlayerCombat))]
    [RequireComponent(typeof(Damageable))]
    public class PlayerStateMachine : MonoBehaviour
    {
        [Header("Required References")]
        [field: SerializeField] public PlayerInputReader PlayerInput { get; private set; }
        [field: SerializeField] public PlayerMotor PlayerMotor { get; private set; }
        [field: SerializeField] public PlayerAnimator PlayerAnimator { get; private set; }
        [field: SerializeField] public PlayerCombat PlayerCombat { get; private set; }
        [field: SerializeField] public Damageable PlayerDamageable { get; private set; }

        public PlayerBaseState CurrentState { get; private set; }
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerJumpState JumpState { get; private set; }
        public PlayerFallState FallState { get; private set; }
        public PlayerRollState RollState { get; private set; }
        public PlayerSwordAttackState SwordAttackState { get; private set; }

        public void SwitchState(PlayerBaseState newState)
        {
            if (newState == null || newState == CurrentState)
            {
                return;
            }

            CurrentState?.ExitState();
            CurrentState = newState;
            CurrentState.EnterState();
        }

        private void Awake()
        {
            LogRequiredReferences();

            IdleState = new PlayerIdleState(this);
            MoveState = new PlayerMoveState(this);
            JumpState = new PlayerJumpState(this);
            FallState = new PlayerFallState(this);
            RollState = new PlayerRollState(this);
            SwordAttackState = new PlayerSwordAttackState(this);
        }

        private void Start()
        {
            if (!HasRequiredReferences())
            {
                Debug.LogError("PlayerStateMachine cannot start because one or more required references are missing.", this);
                enabled = false;
                return;
            }

            SwitchState(IdleState);
        }

        private void Update()
        {
            PlayerInput.ReadInput();

            if (CurrentState != RollState)
            {
                PlayerMotor.Turn(PlayerInput.TurnInput);
            }

            CurrentState.UpdateState();
            CurrentState.CheckSwitchStates();
        }

        private bool HasRequiredReferences()
        {
            return PlayerInput != null
                && PlayerMotor != null
                && PlayerAnimator != null
                && PlayerCombat != null
                && PlayerDamageable != null;
        }

        private void LogRequiredReferences()
        {
            LogRequiredReference(PlayerInput, nameof(PlayerInput), typeof(PlayerInputReader).Name);
            LogRequiredReference(PlayerMotor, nameof(PlayerMotor), typeof(PlayerMotor).Name);
            LogRequiredReference(PlayerAnimator, nameof(PlayerAnimator), typeof(PlayerAnimator).Name);
            LogRequiredReference(PlayerCombat, nameof(PlayerCombat), typeof(PlayerCombat).Name);
            LogRequiredReference(PlayerDamageable, nameof(PlayerDamageable), typeof(Damageable).Name);
        }

        private void LogRequiredReference(Object reference, string fieldName, string componentName)
        {
            if (reference == null)
            {
                Debug.LogError($"PlayerStateMachine requires {componentName} assigned to '{fieldName}' on {name}.", this);
                return;
            }

            Debug.Log($"PlayerStateMachine found required {componentName} reference on {name}.", this);
        }
    }
}

