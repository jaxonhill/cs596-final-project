using Cysharp.Threading.Tasks;
using NPCs.Enemies;
using UnityEngine;

namespace NPCs.States.AttackStates
{
    public class RangedAttackState : AttackState
    {
        private readonly RangedEnemy enemy;
        
        public RangedAttackState(RangedEnemy new_enemy) : base(new_enemy) { enemy = new_enemy; }
    
        protected override async UniTask DoAttack()
        {
            // Projectile spawns in front of the ranged enemy, with a distance specified by the SpawnOffest
            var newPos = npc.transform.position + enemy.OffsetAmount;
            
            enemy.SetAnimationTrigger("Attack");

            await UniTask.WaitUntil(() => enemy.nowThrow);

            enemy.nowThrow = false;
            
            // Instantiate the projectile at this new position
            var projectile = Object.Instantiate(enemy.Projectile, newPos, Quaternion.identity).GetComponent<Projectile>();
            projectile.Init(enemy); // Initialize the projectile (using this enemy)
        }
    }
}
