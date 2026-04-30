using NPCs.States;
using UnityEngine;

namespace NPCs.Enemies 
{
    /// <summary> The abstract class for defining enemies </summary>
    public abstract class BaseEnemy : NPC
    {
        /// <summary> Object used for storing information on an enemy's patrolling </summary>
        [SerializeField] private PatrolObject patrolObject = new();
        
        
        [SerializeField] private GameObject player;
        
        
        // Create a new patrol state for this enemy, and set its NPC Idle State to this patrol state. Then do normal NPC Start() implementation
        protected new void Start() {
            PatrolState patrolState = new (this);
            idleState = patrolState;
            GlobalGameManager.AddEnemy(transform);
            base.Start(); }
        
        
        // HEADER: GET Methods
        
        /// <summary> Return the Patrol Object for this enemy </summary>
        public PatrolObject GetPatrolObject() { return patrolObject; }
        
        public Vector3 GetPosition() {return position;}
    }
    
}
