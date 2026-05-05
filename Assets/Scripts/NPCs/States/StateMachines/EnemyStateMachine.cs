using Cysharp.Threading.Tasks;
using NPCs.Enemies;
using NPCs.States.AttackStates;
using NPCs.States.ChaseStates;
using NPCs.States.IdleStates;
using static NPCs.States.NPCState;
    
namespace NPCs.States.StateMachines
{
    public class EnemyStateMachine : NPCStateMachine
    {
        
        /* * * * * * * * * *
         * Constant Fields *
         * * * * * * * * * */
        private readonly BaseEnemy enemy;
        
        
        // HEADER: CONSTRUCTOR
        
        public EnemyStateMachine(BaseEnemy this_enemy) : base(this_enemy) { enemy = this_enemy; }
        
        
        // HEADER: STATE MANAGEMENT

        public override async UniTask ChangeToState(NPCStateEnum state) {
            if (state == NPCStateEnum.Searching){ await ChangeToSearchState(); return;}
            await base.ChangeToState(state);
        }
        
        protected override UniTask ChangeToIdleState() {
            currentState = new PatrolState(enemy);
            return UniTask.CompletedTask;
        }
        
        /// Change to a <see cref="SearchState">Search State</see>
        private UniTask ChangeToSearchState() {
            currentState = new SearchState(enemy);
            return UniTask.CompletedTask;
        }
        
        protected override UniTask ChangeToChaseState() {
            if (enemy as RangedEnemy) { currentState = new RangedEnemyChaseState(enemy as RangedEnemy); }
            else { currentState = new EnemyChaseState(enemy); }
            return UniTask.CompletedTask;
        }

        protected override UniTask ChangeToAttackState()
        {
            currentState = enemy switch
            {
                RangedEnemy rangedEnemy => new RangedAttackState(rangedEnemy),
                MeleeEnemy meleeEnemy => new MeleeAttackState(meleeEnemy),
                _ => currentState
            };
            return UniTask.CompletedTask;
        }
    }
}
