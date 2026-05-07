namespace Player.States
{
    public abstract class PlayerBaseState
    {
        protected readonly PlayerStateMachine player;

        protected PlayerBaseState(PlayerStateMachine currentContext)
        {
            player = currentContext;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
        public abstract void CheckSwitchStates();
    }
}
