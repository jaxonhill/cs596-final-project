using Components;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NPCs.States
{
    public class DamagedState : NPCState
    {
        // HEADER: CONSTRUCTOR
        
        public DamagedState(NPC npc) : base(npc) {}

        
        // HEADER: STATE METHODS
        
        // ReSharper disable Unity.PerformanceAnalysis
        public override async UniTask Enter() {
            
            // If HP reaches 0, die
            if (health.GetValue() <= 0) { await stateMachine.ChangeToState(NPCStateEnum.Death); }

            movement.SetValue(0); // Stun the NPC temporarily
            
            await npc.SetAnimationTrigger("Damage");
            
            // If the NPC's target still lives, return to Chasing State
            if (npc.Target != null) { await stateMachine.ChangeToState(NPCStateEnum.Chasing); return; }
            
            // Otherwise, return to Idle State
            await stateMachine.ChangeToState(NPCStateEnum.Idle);
        }

        public override UniTask Run() { return UniTask.CompletedTask; }

        public override UniTask Exit()
        { return UniTask.CompletedTask; }
    }
}
