using Cysharp.Threading.Tasks;

namespace NPCs.States
{
    public class IdleState : NPCState
    {
        public override void Enter() { throw new System.NotImplementedException(); }

        public override UniTask Run() { return UniTask.CompletedTask; }
        
        public override void Exit() { throw new System.NotImplementedException(); }
    }
}
