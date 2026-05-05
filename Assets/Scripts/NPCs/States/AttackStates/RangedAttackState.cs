using Cysharp.Threading.Tasks;
using NPCs.Enemies;
using UnityEngine;

namespace NPCs.States.AttackStates
{
    public class RangedAttackState : AttackState
    {
        private RangedEnemy enemy;
        
        public RangedAttackState(RangedEnemy new_enemy) : base(new_enemy)
        {
            enemy = new_enemy;
        }
    
        protected override UniTask DoAttack()
        {
            var newPos = npc.transform.position +npc.transform.forward * enemy.SpawnOffset;

            var projectile = Object.Instantiate(enemy.Projectile, newPos, Quaternion.identity).GetComponent<Projectile>();
            projectile.Init(enemy);
            
            return UniTask.CompletedTask;
        }
    }
}
