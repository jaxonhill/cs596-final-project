using Components;
using Cysharp.Threading.Tasks;
using GameManaging;
using NPCs.Enemies;
using UnityEngine;

namespace NPCs.States
{
    public class DieState : NPCState
    {
        public DieState(NPC npc) : base(npc) {}
        
        private Movement movement;
        
        // ReSharper disable Unity.PerformanceAnalysis
        // ReSharper disable once AsyncVoidMethod
        public override async UniTask Enter()
        {
            movement.SetValue(0);
            //await npc.SetAnimationTrigger("Death");
            GlobalGameManager.RemoveEnemy(npc.transform);
            await UniTask.Delay(1000); Exit();
        }

        public override UniTask Run() { return UniTask.CompletedTask; }

        public override UniTask Exit()
        {
            npc.Destroy(); 
            return UniTask.CompletedTask;
        }
    }
}
