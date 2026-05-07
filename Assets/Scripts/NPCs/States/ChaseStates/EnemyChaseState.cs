using NPCs.Enemies;

namespace NPCs.States.ChaseStates
{
    public class EnemyChaseState : ChaseState
    {
        protected readonly BaseEnemy enemy;
        
        
        // HEADER: CONSTRUCTOR
        public EnemyChaseState(BaseEnemy this_enemy) : base(this_enemy) {enemy = this_enemy;}

        
        // HEADER: CHASE METHODS

        /// If the enemy loses a target, search for them
        protected override void IfLost() {
            if(enemy.GetWillSearch()) { _ = stateMachine.ChangeToState(NPCStateEnum.Searching); return; }
            _ = stateMachine.ChangeToState(NPCStateEnum.Idle);
        }
        
    }
    
}
