using System.Collections.Generic;
using System.Linq;
using Components.NPCComponents;
using Cysharp.Threading.Tasks;
using GameManaging;
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
        
        /* * * * * * * * * * *
         * Patrolling Fields *
         * * * * * * * * * * */
        /// Determines the type of patrolling an enemy should perform (Ordered, Random, or Free) <br/> See <see cref="PatrolType">PatrolType</see>
        private PatrolType patrolType;
        /// List of Empty Objects representing the coordinates of patrol destinations
        private List<Transform> patrolMarkers;
        /// How long an enemy should remain at a patrol marker before proceeding to the next
        private int patrolDelay; 
        
        /* * * * * * * * * * * * * * * 
         * Patrolling Runtime Fields *
         * * * * * * * * * * * * * * */
        /// Index representing the current patrol destination in patrol markers
        private int patrolIndex;
        /// <summary> The current path from the Start Node to the Goal Node (from Top to Bottom of the stack)</summary>
        private Stack<Vector3> curPath;
        
        
        // HEADER: CONSTRUCTOR

        public PatrolState(BaseEnemy this_enemy) : base(this_enemy) { enemy = this_enemy; }

        
        // HEADER: STATE METHODS

        // Do Animations & Set Default Values
        public override UniTask Enter()
        {
            // INITIALIZE PATROL DATA
            patrolType = (PatrolType)enemy.GetPatrolType();
            patrolMarkers = enemy.GetPatrolMarkers();
            patrolDelay = enemy.GetPatrolDelay();
            
            enemy.SetAnimationTrigger("Idle");
            
            // SET DEFAULTS
            enemy.SetTarget(NO_TARGET);
            movement.SetValue(movement.defaultSpeed); 
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
            npc.transform.rotation = Quaternion.LookRotation(
                movement.GetDirectionIgnoreY(npc.target.position)); // Look at target before attacking
            enemy.SetAnimationBool("Walk", false);
            await enemy.AwaitAnimationTrigger("Scream");
        }
        
        
        // HEADER: PATROL METHODS
        
        /// <summary> Implementation for making an Enemy follow a given path </summary>
        private void Patrol()
        {
            // If the enemy has not yet reached the target position, move towards it
            if (!NoPath() && !movement.AtDestination())
            {
                enemy.SetAnimationBool("Walk", true);
                movement.MoveTowardsDestination();
            }
            
            // If the enemy has reached the target position, but the path has not been fully traversed, get the next target position
            else if (!NoPath())
            {
                movement.SetDestination(curPath.Pop());
            }

            // If the Stack is empty, make a new path
            else { 
                enemy.SetAnimationBool("Walk", false);
                
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

            if (curPath == NO_PATH) return; // If no path was found, return
            else {  /*? Free Patrol implementation here*/ }
            movement.SetDestination(curPath.Pop()); // Pop the first target position from the path stack
        }

        
        // HEADER: Enemy Detection

        /// Enemy changes to Chase State, and chases the given target 
        private void ChaseTarget(Transform target) {
            enemy.SetTarget(target);
            _ =  stateMachine.ChangeToState(NPCStateEnum.Chasing);
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
        private List<Transform> GetFriendliesInView() {
            // Get all the "Friendlies" who this enemy has a direct line of sight to
            var friendlies_in_sight = enemy.enemies.Where(friendly =>
                detection.TransformInSight(friendly.transform)).ToList();
            
            return friendlies_in_sight;
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
