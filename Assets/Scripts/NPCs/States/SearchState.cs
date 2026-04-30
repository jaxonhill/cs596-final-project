using Cysharp.Threading.Tasks;

namespace NPCs.States
{
    public class SearchState : NPCState
    {
        
        /* * * * * * * * * *
         * NPC Components  *
         * * * * * * * * * */
        private readonly NPC npc;
        
        public SearchState(NPC new_npc) { npc = new_npc; }
        
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