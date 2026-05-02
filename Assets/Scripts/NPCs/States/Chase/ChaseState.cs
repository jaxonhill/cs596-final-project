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

        public ChaseState(NPC new_npc) { npc = new_npc; }

        public override void Enter()
        {
            target = npc.GetTarget();
            npc.movement.SetValue(20);
        }

        public override UniTask Run()
        {
            FollowTarget();

            if (CheckIfInRange())
            {
                npc.ChangeToState(NPCStateEnum.Attacking);
            }
            return UniTask.CompletedTask;
        }

        public override void Exit() { }

        private void FollowTarget() {  npc.movement.MoveTowardsLocation(target.position); }

        private bool CheckIfInRange()
        {
            return npc.movement.AtLocation(target.position);
        }
        
        
    }
}
