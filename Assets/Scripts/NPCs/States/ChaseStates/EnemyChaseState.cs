using NPCs.Enemies;

namespace NPCs.States.ChaseStates
{
    public class EnemyChaseState : ChaseState
    {
        protected readonly BaseEnemy enemy;
        
        
        // HEADER: CONSTRUCTOR
        public EnemyChaseState(BaseEnemy enemy) : base(enemy) {}

        
        // HEADER: CHASE METHODS

        /// If the enemy loses a target, search for them
        protected override async void IfLost()
        {
            if(enemy.GetWillSearch())
            {
                await stateMachine.ChangeToState(NPCStateEnum.Searching);
                return;
            }
            await stateMachine.ChangeToState(NPCStateEnum.Idle);
        }
        
    }
    
}
