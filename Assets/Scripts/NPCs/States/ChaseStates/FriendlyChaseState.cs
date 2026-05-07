using Components;
using Components.NPCComponents;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NPCs.States.ChaseStates
{
    public class FriendlyChaseState : ChaseState
    {
        
        public FriendlyChaseState(NPC npc) : base(npc) {}

        protected override void IfLost() { _ = stateMachine.ChangeToState(NPCStateEnum.Idle); }
    }
}
