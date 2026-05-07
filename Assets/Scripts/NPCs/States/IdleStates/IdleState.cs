using Cysharp.Threading.Tasks;

namespace NPCs.States.IdleStates
{
    /// Default state for NPCs, when they are not chasing, attacking, etc
    public abstract class IdleState : NPCState
    {
        
        protected IdleState(NPC npc) : base (npc){}
        
        public override UniTask Enter() { throw new System.NotImplementedException(); }

        public override UniTask Run() { return UniTask.CompletedTask; }
        
        public override UniTask Exit() { throw new System.NotImplementedException(); }
    }
}
