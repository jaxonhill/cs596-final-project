using System.Collections.Generic;
using System.Linq;
using Combat;
using Components;
using Cysharp.Threading.Tasks;
using GameManaging;
using Tools;
using UnityEngine;

namespace NPCs.States.AttackStates
{
    public class MeleeAttackState : AttackState
    {
        
        /* * * * * * * * * 
         * Runtime Fields *
         * * * * * * * * */
        /// The NPC is in the middle of a melee attack (Like a sword swing)
        private bool attacking;
        /// The transforms that were hit during an attack
        private List<Transform> hits;
        /// Helps determine the size of the attack hitbox 
        private Vector3 halfExtents;

        
        // HEADER: CONSTRUCTOR 
        public MeleeAttackState(NPC npc) : base(npc) {}

        public override UniTask Run()
        {
            if(attacking) SwingAttack();
            return UniTask.CompletedTask;
        }
        
        protected override async UniTask DoAttack()
        {
            // Some initializations 
            hits = new List<Transform>();
            halfExtents = npc.transform.localScale / 2; // Attack hitbox will be the size of the attacker
            
            // Enemy is swinging their sword./l;
            attacking = true;
            await npc.SetAnimationIntTrigger("Attack", Random.Range(1,3));
            attacking = false;
        }

        private void SwingAttack()
        {
            // Use a Boxcast to determine who's in the attack range
            var new_hits = PhyTools.BoxCastAll(position, halfExtents,
                npc.transform.forward, Quaternion.identity, attack.GetRange(), Color.blue);
            foreach (var hit in new_hits.Where(hit => !hits.Contains(hit)))
            {
                if (!hit || !GlobalGameManager.GetTargets(npc.transform).Contains(hit)) continue;
                if (hit.TryGetComponent<Health>(out var hit_health))
                {
                    hit_health.LowerHealth(attack.GetValue());
                    hits.Add(hit);
                }
                else if (hit.TryGetComponent<Damageable>(out var hit_damage))
                {
                    hit_damage.TakeDamage(attack.GetValue(), npc.gameObject);
                    hits.Add(hit);
                }
            }
        }
        
        // HEADER: HELPER METHODS

        private bool ValidTarget(Transform hit)
        {
            return hit // Hit is not null
                   && GlobalGameManager.GetTargetTags(npc.transform).Contains(hit.tag); // Hit transform is an enemy of this NPC
        }
    }
}
