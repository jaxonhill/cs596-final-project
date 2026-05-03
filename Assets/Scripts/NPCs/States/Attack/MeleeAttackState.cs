using System.Collections.Generic;
using System.Linq;
using Components;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NPCs.States.Attack
{
    public class MeleeAttackState : AttackState {
        public MeleeAttackState(NPC this_npc) : base(this_npc) { npc = this_npc; }
        
        
        /* * * * * * *
         * Attacking *
         * * * * * * */
        /// The NPC is in the middle of a melee attack (Like a sword swing)
        protected bool attacking;
        private List<Transform> hits;
        private Vector3 halfExtents;


        public override UniTask Run()
        {
            if(attacking) SwingAttack();
            return UniTask.CompletedTask;
        }
        
        protected override async UniTask DoAttack()
        {
            // Some initializations 
            hits = new List<Transform>();
            halfExtents = transform.localScale / 2;
            
            await UniTask.Delay(attack.GetWindup()); // Wait for the Windup
            
            // Enemy is swinging their sword
            attacking = true;
            await UniTask.Delay(attack.GetLength()); // Amount of time the sword swing should last
            attacking = false;
            
            // Get all entities that were hit in the swing
            foreach (var hit in hits.Where(hit => hit && GlobalGameManager.GetTargetTags(transform).Contains(hit.tag)))
            {
                var health = hit.GetComponent<Health>();
                health.LowerHealth(attack.GetValue());
            }
            
            // Wait for the cooldown
            await UniTask.Delay(attack.GetCooldown());
            
            // If the target is not dead or dying, go back to chasing them (which will change back to an attack state immediately if still in range)
            if (npc.GetTarget() && npc.GetState() is not DieState) {
                npc.ChangeToState(NPCStateEnum.Chasing); return; }
            
            // Otherwise, the enemy goes back to being idle 
            npc.ChangeToState(NPCStateEnum.Idle);
        }

        private void SwingAttack()
        {
            // Use a Boxcast to determine who's in the attack range
            var new_hits = PhyTools.BoxCastAll(position, halfExtents,
                npc.transform.forward, Quaternion.identity, attack.GetRange(), Color.blue);
            foreach (var hit in new_hits.Where(hit => !hits.Contains(hit))) { hits.Add(hit); }
        }
    }
}
