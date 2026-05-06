using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NPCs.States
{
    public class AttackState : NPCState
    {
        
        private NPC npc;

        public AttackState(NPC this_npc)
        {
            npc = this_npc;
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public override async void Enter()
        {
            var size = npc.transform.localScale;
            await UniTask.Delay(1000);
            Physics.BoxCast(npc.GetPosition() + (npc.transform.forward * 2), size/2,
                Vector3.zero, out RaycastHit hit);
            Gizmos.DrawCube(npc.GetPosition() + (npc.transform.forward * 2), size);
            if (hit.transform && hit.transform.CompareTag("Player"))
            {
                // Functionality for player damage here
            }
            await UniTask.Delay(1000);
            if (npc.GetTarget())
            {
                npc.ChangeToState(NPCStateEnum.Chasing);
                return;
            }
            npc.ChangeToState(NPCStateEnum.Idle);
        }

        public override UniTask Run() { return UniTask.CompletedTask; }

        public override void Exit() { }
    }
}
