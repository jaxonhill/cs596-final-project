using Cysharp.Threading.Tasks;
using NPCs.Enemies;
using UnityEngine;

namespace NPCs.States
{
    public class ChaseState : NPCState
    {
        /* * * * * * * * * * *
         * Target Components *
         * * * * * * * * * * */
        protected Transform target;
        
        /* * * * * * * * * *
         * NPC Components  *
         * * * * * * * * * */
        protected readonly NPC npc;

        public ChaseState(NPC new_npc) { npc = new_npc; }

        public override void Enter()
        {
            target = npc.GetTarget();
            npc.SetMovementSpeed(20);
        }

        public override UniTask Run()
        {
            FollowTarget();

            if (CheckIfInRange())
            {
                //npc.ChangeToState(NPCStateEnum.Attacking);
            }
            return UniTask.CompletedTask;
        }

        public override void Exit() { }

        private void FollowTarget() {  npc.MoveTowardsLocation(target.position); }

        private bool CheckIfInRange()
        {
            return npc.AtLocation(target.position);
        }
        
        
    }
}
