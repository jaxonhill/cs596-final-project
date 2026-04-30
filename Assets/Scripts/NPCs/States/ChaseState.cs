using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NPCs.States
{
    public class ChaseState : NPCState
    {
        
        /* * * * * * * * * * *
         * Target Components *
         * * * * * * * * * * */
        private Transform target;
        
        /* * * * * * * * * *
         * NPC Components  *
         * * * * * * * * * */
        private readonly NPC npc;

        public ChaseState(NPC new_npc) { npc = new_npc; }
        
        public override void Enter() { target = npc.GetTarget(); }

        public override UniTask Run()
        {
            FollowTarget();
            if (!CheckIfInSight())
            {
                npc.ChangeToState(NPCStateEnum.Searching);
            }

            if (CheckIfInRange())
            {
                npc.ChangeToState(NPCStateEnum.Attacking);
            }
            return UniTask.CompletedTask;
        }

        public override void Exit() { }

        private void FollowTarget() {  npc.MoveTowardsLocation(target.position); }

        private bool CheckIfInRange()
        {
            return npc.AtLocation(target.position);
        }

        private bool CheckIfInSight()
        {
            Physics.Raycast(npc.GetPosition(), target.position, out var hit, npc.GetDetectionRange());
            return hit.transform && (hit.transform.CompareTag("Player") || hit.transform.CompareTag("Friendly"));
        }
        
    }
}
