using System.Collections.Generic;
using GameManaging;
using NPCs.States.StateMachines;
using TriInspector;
using UnityEngine;


namespace NPCs.Enemies 
{
    /// <summary> The abstract class for defining enemies </summary>
    [RequireComponent(typeof(EnemyStateMachine))]
    public abstract class BaseEnemy : NPC
    {
        /* * * * * * * *
         * Patrolling *
         * * * * * * * */
        // Method specifically used for the Dropdown attribute of patrolType (Creates dropdown list functionality in the inspector)
        private IEnumerable<TriDropdownItem<int>> GetPatrolTypeEnum()
        {
            return new TriDropdownList<int> {
                {"Random", 0},
                {"Ordered", 1},
                {"Free", 2}, };
        }
        
        [Title("Patrolling")] 
        
        [Tooltip("The patrolling type the enemy uses: Ordered, Random, or Free"), Dropdown(nameof(GetPatrolTypeEnum)), SerializeField]
        private int patrolType;
        
        [Tooltip("List of transforms representing the positions that enemies will move to when not chasing the player"), SerializeField] 
        private List<Transform> patrolMarkers;
        
        [Tooltip("How long an enemy should remain at a patrol marker before proceeding to the next"), SerializeField]
        private int patrolDelay;
        
        
        [Title("Searching")] 
        
        [Tooltip("Whether the enemy will search for a lost player (Melee), or return to idle (Range)"), SerializeField] 
        protected bool willSearch;
        
        
        // HEADER: STANDARD METHODS
        
        // Add this enemy to the GlobalGameManager Enemy List
        protected void Start() { GlobalGameManager.AddEnemy(transform); }


        // HEADER: GETTERS
        
        public int GetPatrolType(){ return patrolType;}
        
        public List<Transform> GetPatrolMarkers(){ return patrolMarkers;}
        
        public int GetPatrolDelay(){return patrolDelay;}
        
        public bool GetWillSearch(){return willSearch;}
    }
}
