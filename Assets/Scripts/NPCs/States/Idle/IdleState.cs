using Cysharp.Threading.Tasks;

namespace NPCs.States.Idle
{
    /// Default state for NPCs, when they are not chasing, attacking, etc
    public abstract class IdleState : NPCState
    {
        public override void Enter() { throw new System.NotImplementedException(); }

        public override UniTask Run() { return UniTask.CompletedTask; }
        
        public override void Exit() { throw new System.NotImplementedException(); }
    }
}
