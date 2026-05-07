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
            if (health.GetValue() <= 0) { _ = stateMachine.ChangeToState(NPCStateEnum.Damaged, NPCStateEnum.Death); return; }

            movement.Stop(); // Stun the NPC temporarily
            
            await npc.AwaitAnimationTrigger("Damage");
            
            // If the NPC's target still lives, return to Chasing State
            if (npc.target != null) { _ = stateMachine.ChangeToState(NPCStateEnum.Damaged, NPCStateEnum.Chasing); return; }
            
            // Otherwise, return to Idle State
            _ = stateMachine.ChangeToState(NPCStateEnum.Damaged, NPCStateEnum.Idle);
        }

        public override UniTask Run() { return UniTask.CompletedTask; }

        public override UniTask Exit()
        { return UniTask.CompletedTask; }
    }
}
