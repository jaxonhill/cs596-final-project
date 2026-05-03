using System.Collections.Generic;
using System.Linq;
using Components.NPC;
using Cysharp.Threading.Tasks;
using NPCs.Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPCs.States.Idle
{
    /// Default state for enemies, they wander between specified points looking for their targets
    public class PatrolState : IdleState
    {
        
        /// <summary> Enum representing the type of patrolling an enemy should perform <br/> [
        /// <see cref="PatrolType.Random"> Random </see>,
        /// <see cref="PatrolType.Ordered"> Ordered </see>,
        /// <see cref="PatrolType.Free"> Free </see> ] </summary>
        private enum PatrolType
        {
            /// Enemy chooses a random patrol marker and moves to it
            Random = 0, 
            /// Enemy moves in order from one patrol marker to the next, and starts over upon reaching the last one
            Ordered = 1, // Enemy moves in order from one patrol marker to the next, and starts over upon reaching the last one
            Free = 2 //! Not implemented - Four patrol markers designate a square that an enemy can move freely in
        }
        
        
        /* * * * * * * * * * *
         * Enemy Components  *
         * * * * * * * * * * */
        /// The primary script for enemies: <see cref="BaseEnemy">BaseEnemy</see>
        private readonly BaseEnemy enemy;
        private Vector3 position => enemy.transform.position;
        /// Component that controls movement: <see cref="NPCMovement">NPCMovement</see>
        private NPCMovement movement;
        /// Component that controls how NPCs detect enemies: <see cref="Detection">NPCMovement</see>
        private Detection detection;
        
        
        /* * * * * * * * * * *
         * Patrolling Fields *
         * * * * * * * * * * */
        /// Determines the type of patrolling an enemy should perform (Ordered, Random, or Free) <br/> See <see cref="PatrolType">PatrolType</see>
        private readonly PatrolType patrolType;
        /// List of Empty Objects representing the coordinates of patrol destinations
        private readonly List<Transform> patrolMarkers;
        /// How long an enemy should remain at a patrol marker before proceeding to the next
        private readonly int patrolDelay;
        
        /* * * * * * * * * * * * * * * 
         * Patrolling Runtime Fields *
         * * * * * * * * * * * * * * */
        /// Index representing the current patrol destination in patrol markers
        private int patrol_index;
        /// <summary> The current path from the Start Node to the Goal Node (from Top to Bottom of the stack)</summary>
        private Stack<Vector3> curPath;
        
        
        // HEADER: CONSTRUCTOR

        public PatrolState(BaseEnemy enemy)
        {
            this.enemy = enemy;
            
            patrolType = (PatrolType)enemy.GetPatrolType();
            patrolMarkers = enemy.GetPatrolMarkers();
        }

        
        // HEADER: STATE METHODS

        // Set Default Values
        public override void Enter()
        {
            
            movement = enemy.GetComponent<NPCMovement>();
            detection = enemy.GetComponent<Detection>();
            
            // Defaults
            enemy.SetTarget(null); // Enemy has no target
            movement.SetValue(10); // Speed set to 10
            // Start at first patrol marker 
            patrol_index = 0; 
            SetNewPath(); 
        }

        // Run State Functionality
        public override UniTask Run()
        {
            FindPrimaryTarget(); // Look for a target
            if (!pause) Patrol(); // If patrolling is not paused, patrol
            return UniTask.CompletedTask;
        }
        
        public override void Exit() { }
        
        
        // HEADER: PATROL METHODS
        
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary> Implementation for making an Enemy follow a given path </summary>
        private void Patrol()
        {
            // If the enemy has not yet reached the target position, move towards it
            if (!movement.AtDestination()) { movement.MoveTowardsDestination(); }
            
            // If the enemy has reached the target position, but the path has not been fully traversed, get the next target position
            else if (!NoPath()){ movement.SetDestination(curPath.Pop()); /* Pop the next target position from the current path */ }

            // If the Stack is empty, make a new path
            else { _ = InvokeWithPause(SetNewPath, patrolDelay); }
        }
        
        /// <summary> Implementation for creating a new Enemy Path, based on the Patrol Type </summary>
        private void SetNewPath()
        {
            if (patrolType is PatrolType.Random or PatrolType.Ordered)
            {
                // If Patrol Type Random, Set a random patrol destination from patrol markers to the goal destination
                if(patrolType == PatrolType.Random) patrol_index = IncrementIndex(patrol_index, patrolMarkers, false, true); 
                
                curPath = CreatePath(); // Create a new path in either case
                
                // If Patrol Type Ordered, increment the index so that the next destination is set for the next SetNextPath call
                if(patrolType == PatrolType.Ordered)patrol_index = IncrementIndex(patrol_index, patrolMarkers); // Increment patrol index, loop back to 0 when list size is reached
            }
            else {  /*? Free Patrol implementation here*/ }
            movement.SetDestination(curPath.Pop()); // Pop the first target position from the path stack
        }

        
        // HEADER: Enemy Detection

        /// Enemy changes to Chase State, and chases the given target 
        private void ChaseTarget(Transform target)
        {
            enemy.SetTarget(target);
            enemy.ChangeToState(NPCStateEnum.Chasing);
        }
        
        /// Iterate through a list of visible enemies to determine which one is the closest (enemy will be set to target this one)
        private void FindPrimaryTarget()
        {
            var targets = GetFriendliesInView();
            
            if (targets.Count <= 0) return; // If no targts found, continue
            // Initialize target & min_distance with default values
            Transform target = null;  float min_distance = int.MaxValue; 
            // Iterate through the targets, find the one with the smallest distance
            foreach (var t in targets)
            {
                var dist = Vector3.Distance(position, t.position);
                if (!(dist < min_distance)) continue; // If the distance isn't less, disregard this target
                target = t; min_distance = dist;
            }
            if (target) { ChaseTarget(target); } // If a target exists, set the enemy to chase this target
        }

        /// Iterate through all potential enemies (as declared in the <see cref="GlobalGameManager">GlobalGameManager</see>)
        /// to see if any are in sight of this enemy
        private List<Transform> GetFriendliesInView()
        {
            // Grab all friendlies in front of this enemy
            var friendlies_in_front = enemy.GetEnemies().Where(friendly => movement.InFront(friendly.position));
            // From that list, Grab all enemies in view of the enemy (no obstructions)
            var friendlies_in_view = friendlies_in_front.Where(friendly => 
                PhyTools.RaycastForTag(position, movement.GetDirection(friendly.position), 
                detection.GetValue(), new List<string>() {"Friendly", "Player"}, Color.red)).ToList();
            return friendlies_in_view;
        }
        
        
        // HEADER: HELPER METHODS
        // HDESC: Just some methods to help make the code more readable
        
        /// <summary> Returns true if the enemy doesn't have a path / curPath is empty </summary>
        private bool NoPath() { return curPath.Count == 0; }

        /// <summary> Use A* search to create a path to the enemy's next destination</summary>
        private Stack<Vector3> CreatePath() {  return AStarSearch.Search(enemy.transform, GetDestination()); }

        /// <summary> Return the vector coordinates of the enemy's current target destination </summary>
        private Vector3 GetDestination() { return patrolMarkers[patrol_index].position; }

        /// <summary> Method for incrementing an array's index </summary>     <param name="index">Array Index</param>
        /// <param name="enumerable"> The array / list of the index </param>
        /// <param name="loop"> Whether the index should loop back to 0. If not, just return -1</param>
        /// <param name="random"> Whether or not a random index should be chosen, rather than incrementing </param>
        private static int IncrementIndex<T>(int index, IEnumerable<T> enumerable, bool loop = true, bool random = false)
        {
            // If Random, return a random index (within the size of the list)
            if (random) { return Random.Range(0, enumerable.Count()); }

            // If the index has not reached the list size, increment it
            if (index < enumerable.Count() - 1) return ++index;
            
            // If Loop (and index has reached list size), return 0
            if(loop){return 0;} 
            
            // If none of these are true, return -1 
            return -1;
        }
    }
}
