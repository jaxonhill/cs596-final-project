using System.Collections.Generic;
using NPCs.States;
using NPCs.States.Attack;
using NPCs.States.Chase;
using NPCs.States.Idle;
using TriInspector;
using UnityEngine;
using static NPCs.States.NPCState;


namespace NPCs.Enemies 
{
    /// <summary> The abstract class for defining enemies </summary>
    public abstract class BaseEnemy : NPC
    {
        /// <summary> The state the enemy enters after their chasing state when they've lost their target </summary>
        private SearchState searchState;
        
        // Method specifically used for the Dropdown attribute of patrolType (Creates dropdown list functionality in the inspector)
        private IEnumerable<TriDropdownItem<int>> GetPatrolTypeEnum()
        {
            return new TriDropdownList<int> {
                {"Random", 0},
                {"Ordered", 1},
                {"Free", 2}, };
        }
        
        /* * * * * * * *
         * Patrolling *
         * * * * * * * */
        [Title("Patrolling")] 
        [Tooltip("The patrolling type the enemy uses: Ordered, Random, or Free"), Dropdown(nameof(GetPatrolTypeEnum)), SerializeField]
        private int patrolType;
        [Tooltip("List of transforms representing the positions that enemies will move to when not chasing the player"), SerializeField] 
        private List<Transform> patrol_markers;
        [Tooltip("How long an enemy should remain at a patrol marker before proceeding to the next"), SerializeField]
        private int patrol_delay;
        
        
        // HEADER: STANDARD METHODS
        
        // Initialize the IdleState as PatrolState, and the ChaseState as EnemyChaseState, as well as the enemy exclusive SearchState
        // Add this enemy to the GlobalGameManager
        protected new void Start() {
            idleState = new PatrolState(this);
            chaseState = new EnemyChaseState(this);
            searchState = new(this);
            
            GlobalGameManager.AddEnemy(transform);
            base.Start(); }
        
        
        // HEADER: STATE METHODS
        
        /// Override of <see cref="NPC.ChangeToState">NPC.ChangeToState()</see> that includes SearchState as a state
        public new void ChangeToState(NPCStateEnum newState)
        {
            if (newState == NPCStateEnum.Searching) { state.Exit(); state = searchState; state.Enter(); }
            else { base.ChangeToState(newState); } // If not search state, do normal NPC.ChangeToState()
        }
        
        
        // HEADER: GETTERS
        
        public int GetPatrolType(){ return patrolType;}
        
        public List<Transform> GetPatrolMarkers(){ return patrol_markers;}
        
        public int GetPatrolDelay(){return patrol_delay;}
    }
}
