using System.Collections.Generic;
using System.Linq;
using Components.NPCComponents;
using Cysharp.Threading.Tasks;
using NPCs.Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPCs.States.IdleStates
{
    /// Default state for enemies, they wander between specified points looking for their targets
    public class PatrolState : IdleState
    {
        /* * * * * * *
         * CONSTANTS  *
         * * * * * * */
        private const Stack<Vector3> NO_PATH = null;
        private const Transform NO_TARGET = null;
        private const int FIRST_MARKER = 0;
        
        
        /* * * * * * *
         * Enums  *
         * * * * * * */
        /// <summary> Enum representing the type of patrolling an enemy should perform <br/> [
        /// <see cref="PatrolType.Random"> Random </see>,
        /// <see cref="PatrolType.Ordered"> Ordered </see>,
        /// <see cref="PatrolType.Free"> Free </see> ] </summary>
        private enum PatrolType
        {
            /// Enemy chooses a random patrol marker and moves to it
            Random = 0, 
            /// Enemy moves in order from one patrol marker to the next, and starts over upon reaching the last one
            Ordered = 1, 
            ///! NOT IMPLEMENTED - Four patrol markers designate a square that an enemy can move freely in
            Free = 2
        }
        
        
        /* * * * * * * * * * *
         * Enemy Components  *
         * * * * * * * * * * */
        /// The primary script for enemies: <see cref="BaseEnemy">BaseEnemy</see>
        private readonly BaseEnemy enemy;
        private Vector3 position => enemy.transform.position;
        /// Component that controls movement: <see cref="NPCMovement">NPCMovement</see>
        private NPCMovement movement => (NPCMovement) enemy.movement;
        /// Component that controls how NPCs detect enemies: <see cref="Detection">NPCMovement</see>
        private Detection detection => enemy.detection;
        
        /* * * * * * * * * * *
         * Patrolling Fields *
         * * * * * * * * * * */
        /// Determines the type of patrolling an enemy should perform (Ordered, Random, or Free) <br/> See <see cref="PatrolType">PatrolType</see>
        private readonly PatrolType patrolType;
        /// List of Empty Objects representing the coordinates of patrol destinations
        private readonly List<Transform> patrolMarkers;
        /// How long an enemy should remain at a patrol marker before proceeding to the next
        private int patrolDelay; // TODO: MAY NEED TO FIX
        
        /* * * * * * * * * * * * * * * 
         * Patrolling Runtime Fields *
         * * * * * * * * * * * * * * */
        /// Index representing the current patrol destination in patrol markers
        private int patrolIndex;
        /// <summary> The current path from the Start Node to the Goal Node (from Top to Bottom of the stack)</summary>
        private Stack<Vector3> curPath;
        
        
        // HEADER: CONSTRUCTOR

        public PatrolState(BaseEnemy enemy) : base(enemy)
        {
            // INITIALIZE PATROL DATA
            patrolType = (PatrolType)enemy.GetPatrolType();
            patrolMarkers = enemy.GetPatrolMarkers();
        }

        
        // HEADER: STATE METHODS

        // Do Animations & Set Default Values
        public override UniTask Enter()
        {
            // TODO: FINISH ANIMATION STUFF
            enemy.SetAnimationTrigger("Idle");
            
            // SET DEFAULTS
            enemy.SetTarget(NO_TARGET);
            movement.SetValue(movement.DefaultSpeed); 
            patrolIndex = FIRST_MARKER; 
            SetNewPath(); // Create a path to the first marker
            
            return UniTask.CompletedTask;
        }

        // Run State Functionality
        public override UniTask Run()
        {
            FindPrimaryTarget(); // Look for a target
            
            if (!pause) Patrol(); // If patrolling is not paused, patrol
            
            return UniTask.CompletedTask;
        }

        // Do Exit Functionality and Exit Animations
        public override async UniTask Exit() {
            movement.Stop();
            // TODO: FIX ANIMATIONS
            await enemy.SetAnimationTrigger("Chase", true);
        }
        
        
        // HEADER: PATROL METHODS
        
        /// <summary> Implementation for making an Enemy follow a given path </summary>
        private void Patrol()
        {
            // If the enemy has not yet reached the target position, move towards it
            if (!movement.AtDestination()) { movement.MoveTowardsDestination(); }
            
            // If the enemy has reached the target position, but the path has not been fully traversed, get the next target position
            else if (!NoPath()){ movement.SetDestination(curPath.Pop()); }

            // If the Stack is empty, make a new path
            else { 
                // TODO: FIX ANIMATIONS
                enemy.SetAnimationTrigger("Idle");
                
                _ = InvokeWithPause(SetNewPath, patrolDelay);
            }
        }
        
        /// <summary> Implementation for creating a new Enemy Path, based on the Patrol Type </summary>
        private void SetNewPath()
        {
            if (patrolType is PatrolType.Random or PatrolType.Ordered)
            {
                // If Patrol Type Random, Set a random patrol destination from patrol markers to the goal destination
                if(patrolType == PatrolType.Random) patrolIndex = IncrementIndex(patrolIndex, patrolMarkers, false, true); 
                
                curPath = CreatePath(); // Create a new path in either case
                
                // If Patrol Type Ordered, increment the index so that the next destination is set for the next SetNextPath call
                if(patrolType == PatrolType.Ordered) patrolIndex = IncrementIndex(patrolIndex, patrolMarkers); // Increment patrol index, loop back to 0 when list size is reached
            }
            else {  /*? Free Patrol implementation here*/ }
            // TODO: FIX ANIMATIONS
            enemy.SetAnimationTrigger("Walk");
            movement.SetDestination(curPath.Pop()); // Pop the first target position from the path stack
        }

        
        // HEADER: Enemy Detection

        /// Enemy changes to Chase State, and chases the given target 
        private async void ChaseTarget(Transform target) {
            enemy.SetTarget(target);
            await stateMachine.ChangeToState(NPCStateEnum.Chasing);
        }
        
        /// Iterate through a list of visible enemies to determine which one is the closest (enemy will be set to target this one)
        private void FindPrimaryTarget()
        {
            var targets = GetFriendliesInView();
            if (Empty(targets)) return; // If no targets found, continue
            
            // Initialize target & min_distance with default values
            var target = NO_TARGET;  float min_distance = int.MaxValue; 
            
            // Iterate through the targets, find the one with the smallest distance
            foreach (var t in targets)
            {
                var dist = Vector3.Distance(position, t.position);
                if (dist >= min_distance) continue; // If the distance isn't less than minDistance, disregard this target
                target = t; min_distance = dist;
            }
            if (target) { ChaseTarget(target); } // If a target exists, set the enemy to chase this target
        }

        /// Iterate through all potential enemies (as declared in the <see cref="GlobalGameManager">GlobalGameManager</see>)
        /// to see if any are in sight of this enemy
        private List<Transform> GetFriendliesInView()
        {
            // Grab all friendlies in front of this enemy
            var friendlies_in_front = enemy.Enemies.Where(friendly => movement.InFront(friendly.position));
            
            // From that list, Grab all enemies in view of the enemy (no obstructions)
            var friendlies_in_view = friendlies_in_front.Where(friendly => 
                PhyTools.RaycastForTag(position, movement.GetDirection(friendly.position), 
                detection.GetValue(), new List<string>() {"Friendly", "Player"}, Color.red)).ToList();
            
            return friendlies_in_view;
        }
        
        
        // HEADER: HELPER METHODS
        // HDESC: Just some methods to help make the code more readable
        
        /// Returns true if a list is empty
        private static bool Empty<T>(IEnumerable<T> list) { return !list.Any(); }
        
        /// Returns true if the enemy doesn't have a path / curPath is empty 
        private bool NoPath() { return curPath == NO_PATH || Empty(curPath); }

        /// Use A* search to create a path to the enemy's next destination
        private Stack<Vector3> CreatePath() {  return AStarSearch.Search(enemy.transform, GetDestination()); }

        /// <summary> Return the vector coordinates of the enemy's current target destination </summary>
        private Vector3 GetDestination() { return patrolMarkers[patrolIndex].position; }

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
