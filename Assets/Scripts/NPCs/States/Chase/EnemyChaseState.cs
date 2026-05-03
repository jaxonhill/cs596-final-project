using Components.NPC;
using Cysharp.Threading.Tasks;
using NPCs.Enemies;
using UnityEngine;

namespace NPCs.States.Chase
{
    public class EnemyChaseState : ChaseState
    {
        private readonly BaseEnemy enemy;
        
        
        // HEADER: CONSTRUCTOR
        public EnemyChaseState(BaseEnemy new_enemy) : base(new_enemy) { enemy = new_enemy; }

        
        // HEADER: CHASE METHODS
        
        /// If the enemy loses a target, search for them
        protected override void IfLost() { enemy.ChangeToState(NPCStateEnum.Searching); }
        
    }
    
}
