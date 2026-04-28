public abstract class PlayerBaseState
{
    protected readonly PlayerStateMachine Ctx;

    protected PlayerBaseState(PlayerStateMachine currentContext)
    {
        Ctx = currentContext;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
}
