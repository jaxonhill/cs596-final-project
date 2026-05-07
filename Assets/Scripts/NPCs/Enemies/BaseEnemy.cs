using NPCs.States;
using NPCs.States.Chase;
using NPCs.States.Idle;
using static NPCs.States.NPCState;
using UnityEngine;

namespace NPCs.Enemies 
{
    /// <summary> The abstract class for defining enemies </summary>
    public abstract class BaseEnemy : NPC
    {
        /// <summary> Object used for storing information on an enemy's patrolling </summary>
        [SerializeField] private PatrolObject patrolObject = new();
        
        /// <summary> The state the enemy is in when not attacking/chasing/etc </summary>
        private SearchState searchState;
        
        [SerializeField] private GameObject player;
        
        
        // Create a new patrol state for this enemy, and set its NPC Idle State to this patrol state. Then do normal NPC Start() implementation
        protected new void Start() {
            PatrolState patrolState = new (this);
            idleState = patrolState;
            EnemyChaseState eChaseState = new(this);
            chaseState = eChaseState;
            searchState = new(this);
            
            GlobalGameManager.AddEnemy(transform);
            base.Start(); }
        
        public new void ChangeToState(NPCStateEnum newState)
        {
            if (newState == NPCStateEnum.Searching)
            {
                state.Exit();
                state = searchState;
                state.Enter();
            }
            else
            {
                base.ChangeToState(newState);
            }
            
        }
        
        // HEADER: GET Methods
        
        /// <summary> Return the Patrol Object for this enemy </summary>
        public PatrolObject GetPatrolObject() { return patrolObject; }
    }
    
}
