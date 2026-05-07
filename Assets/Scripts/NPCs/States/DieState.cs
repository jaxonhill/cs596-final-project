using Cysharp.Threading.Tasks;
using NPCs.Enemies;

namespace NPCs.States
{
    public class DieState : NPCState
    {
        
        public DieState(NPC new_npc) { npc = new_npc; }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public override async void Enter()
        {
            GlobalGameManager.RemoveEnemy(npc.transform);
            await UniTask.Delay(1000);
            Exit();
        }

        public override UniTask Run() { return UniTask.CompletedTask; }

        public override void Exit()
        {
            Destroy(npc);
        }
    }
}
