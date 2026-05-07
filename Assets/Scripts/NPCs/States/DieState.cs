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
        // ReSharper disable Unity.PerformanceAnalysis
        // ReSharper disable once AsyncVoidMethod
        public override async UniTask Enter()
        {
            movement.SetValue(0);
            await npc.AwaitAnimationTrigger("Death");
            GlobalGameManager.RemoveEnemy(npc.transform);
            await UniTask.Delay(1500); 
            Exit();
        }

        public override UniTask Run() { return UniTask.CompletedTask; }

        public override UniTask Exit()
        {
            npc.Destroy(); 
            return UniTask.CompletedTask;
        }
    }
}
