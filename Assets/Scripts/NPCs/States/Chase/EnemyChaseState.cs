using Cysharp.Threading.Tasks;
using NPCs.Enemies;
using UnityEngine;

namespace NPCs.States.Chase
{
    public class EnemyChaseState : ChaseState
    {

        private BaseEnemy enemy;

        public EnemyChaseState(BaseEnemy new_enemy) : base(new_enemy)
        {
            enemy = new_enemy;
        }

        public override UniTask Run()
        {
            base.Run(); 
            if (!CheckIfInSight())  enemy.ChangeToState(NPCStateEnum.Searching);
            return UniTask.CompletedTask;
        }
        
        private bool CheckIfInSight()
        {
            Physics.Raycast(npc.GetPosition(), target.position, out var hit, npc.GetDetectionRange());
            return hit.transform && hit.transform == target;
        }
    }
    
}
