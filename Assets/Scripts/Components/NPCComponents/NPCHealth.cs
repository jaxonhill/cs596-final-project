using System;
using NPCs;
using NPCs.States;

namespace Components.NPCComponents
{
    public class NPCHealth : Health
    {

        private NPC npc;

        public void Start() { npc = GetComponent<NPC>(); }

        public override async void OnDamaged(int value, bool ignoreIFrames = false)
        {
            base.OnDamaged(value, ignoreIFrames);
            await npc.stateMachine.ChangeToState(NPCState.NPCStateEnum.Damaged);
        }
    }
}
