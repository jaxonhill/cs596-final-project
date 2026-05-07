using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NPCs.States.Chase
{
    public class FriendlyChaseState : ChaseState
    {
        public FriendlyChaseState(NPC new_npc) : base(new_npc)
        {
        }

        public override UniTask Run()
        {
            base.Run();
            if (!CheckIfInSight())  npc.ChangeToState(NPCStateEnum.Idle);
            return UniTask.CompletedTask;
        }
        
        private bool CheckIfInSight()
        {
            Physics.Raycast(npc.GetPosition(), target.position, out var hit, npc.GetDetectionRange());
            return hit.transform && hit.transform.CompareTag("Enemy");
        }
    }
}
