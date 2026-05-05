using Components;
using Components.NPCComponents;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NPCs.States.ChaseStates
{
    public class FriendlyChaseState : ChaseState
    {
        
        private Vector3 position => npc.transform.position;
        
        public FriendlyChaseState(NPC npc) : base(npc)
        {
        }

        public override UniTask Run()
        {
            base.Run();
            if (!CheckIfInSight())  stateMachine.ChangeToState(NPCStateEnum.Idle);
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
