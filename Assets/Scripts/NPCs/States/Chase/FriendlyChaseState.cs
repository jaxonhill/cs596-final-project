using Components.NPC;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NPCs.States.Chase
{
    public class FriendlyChaseState : ChaseState
    {
        
        private Vector3 position => npc.transform.position;
        
        private Detection detection => npc.GetComponent<Detection>();
        
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
            Physics.Raycast(position, target.position, out var hit, detection.GetValue());
            return hit.transform && hit.transform.CompareTag("Enemy");
        }

        protected override void IfLost()
        {
            
        }
    }
}
