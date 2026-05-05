using System.Collections.Generic;
using System.Linq;
using Components;
using Cysharp.Threading.Tasks;
using NPCs.Enemies;
using UnityEngine;

namespace NPCs.States.AttackStates
{
    public class MeleeAttackState : AttackState {
        public MeleeAttackState(MeleeEnemy enemy) : base(enemy) {}
        
        /* * * * * * *
         * Attacking *
         * * * * * * */
        /// The NPC is in the middle of a melee attack (Like a sword swing)
        private bool attacking;
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
            
            // Enemy is swinging their sword
            attacking = true;
            await npc.SetAnimationInt("Attack", Random.Range(1,3));
            
            attacking = false;
            
            // Get all entities that were hit in the swing
            foreach (var hit in hits.Where(hit => hit && GlobalGameManager.GetTargetTags(transform).Contains(hit.tag)))
            {
                var health = hit.GetComponent<Health>();
                health.LowerHealth(attack.GetValue());
            }
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
