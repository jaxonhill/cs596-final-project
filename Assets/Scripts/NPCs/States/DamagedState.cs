using Components;
using Cysharp.Threading.Tasks;

namespace NPCs.States
{
    public class DamagedState : NPCState
    {
        /* * * * * * * * * *
         * NPC Components  *
         * * * * * * * * * */
        private Health health;
        private Movement movement;
        
        
        // HEADER: CONSTRUCTOR
        
        public DamagedState(NPC new_npc) { npc = new_npc; }

        
        // HEADER: STATE METHODS
        
        // ReSharper disable Unity.PerformanceAnalysis
        public override void Enter() {
            // Initializations
            health = npc.GetComponent<Health>();
            movement = npc.GetComponent<Movement>();
            
            // If HP reaches 0, die
            if (health.GetValue() <= 0) { npc.ChangeToState(NPCStateEnum.Death); }
            
            movement.SetValue(0); // Stun the NPC temporarily
            
            // If the NPC's target still lives, return to Chasing State
            if (npc.GetTarget() != null) { npc.ChangeToState(NPCStateEnum.Chasing); return; }
            
            // Otherwise, return to Idle State
            npc.ChangeToState(NPCStateEnum.Idle);
        }

        public override UniTask Run() { return UniTask.CompletedTask; }

        public override void Exit()
        { }
    }
}
