using NPCs;
using NPCs.States;

namespace Components.NPCComponents
{
    public class NPCHealth : Health
    {
        public override async void OnDamaged(int value, bool ignoreIFrames = false)
        {
            base.OnDamaged(value, ignoreIFrames);
            _ = npc.stateMachine.ChangeToState(NPCState.NPCStateEnum.Any, NPCState.NPCStateEnum.Damaged, true);
        }
    }
}
