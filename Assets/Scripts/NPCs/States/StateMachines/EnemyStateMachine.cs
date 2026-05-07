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
        private BaseEnemy enemy;
        
        
        // HEADER: AWAKE

        protected override void Start()
        {
            enemy = npc as BaseEnemy;
            base.Start();
        }
        
        
        // HEADER: STATE MANAGEMENT

        protected override async UniTask SetState(NPCStateEnum state) {
            if (state == NPCStateEnum.Searching)
            {
                currentStateEnum = NPCStateEnum.Searching;
                await ChangeToSearchState(); return;
            }
            await base.SetState(state);
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
