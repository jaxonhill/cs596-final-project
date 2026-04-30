using Cysharp.Threading.Tasks;

namespace NPCs.States
{
    public class DieState : NPCState
    {
        
        /* * * * * * * * * *
         * NPC Components  *
         * * * * * * * * * */
        private readonly NPC npc;
        
        public DieState(NPC new_npc) { npc = new_npc; }
        
        public override void Enter()
        {
        
        }

        public override UniTask Run()
        {
            return UniTask.CompletedTask;
        }

        public override void Exit()
        {
        }
    }
}
