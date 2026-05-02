using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NPCs.Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPCs.States.Idle
{
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
        
        
        /* * * * * * * *
         * Patrolling *
         * * * * * * * */
        /// Patrol Object holds information on Patrol Type and Patrol Markers
        private readonly PatrolObject patrol;
        /// Determines the type of patrolling an enemy should perform (Ordered, Random, or Free) <br/> See <see cref="PatrolType">PatrolType</see>
        private PatrolType patrolType => (PatrolType)patrol.patrolType;
        /// List of Empty Objects representing the coordinates of patrol destinations
        private List<Transform> patrol_markers => patrol.patrol_markers;
        /// Index representing the current patrol destination in patrol markers
        private int patrol_index;

        private bool pause_patrol;
        
        
        /// <summary> The current path from the Start Node to the Goal Node (from Top to Bottom of the stack)</summary>
        private Stack<Vector3> curPath;
        
        
        /* * * * * * * * * * *
         * Enemy Components  *
         * * * * * * * * * * */
        private readonly BaseEnemy enemy;

        private Vector3 position => enemy.transform.position;
        
        
        public PatrolState(BaseEnemy enemy) {
            this.enemy = enemy;
            patrol = enemy.GetPatrolObject(); }

        
        // HEADER: STATE METHODS

        // Set patrol index to the first patrol position in the given list (Default), and create a new path 
        public override void Enter()
        {
            patrol_index = 0;
            enemy.SetTarget(null);
            SetNewPath();
            enemy.movement.SetValue(10);
        }

        public override UniTask Run()
        {
            FindPrimaryTarget();
            if (!pause_patrol) _ = Patrol();
            return UniTask.CompletedTask;
        }
        
        public override void Exit() { }
        
        
        // HEADER: PATROL METHODS
        
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary> Implementation for making an Enemy follow a given path </summary>
        private async UniTask Patrol()
        {
            // If the enemy has not yet reached the target position, move towards it
            if (!enemy.movement.AtDestination()) { enemy.movement.MoveTowardsDestination(); }
            
            // If the enemy has reached the target position, but the path has not been fully traversed, get the next target position
            else if (!NoPath()){ enemy.movement.SetTarget(curPath.Pop()); /* Pop the next target position from the current path */ }

            // If the Stack is empty, make a new path
            else
            {
                pause_patrol = true;
                await UniTask.Delay(2000); 
                SetNewPath();
                pause_patrol = false;
            }
        }
        
        /// <summary> Implementation for creating a new Enemy Path, based on the Patrol Type </summary>
        private void SetNewPath()
        {
            if (patrolType is PatrolType.Random or PatrolType.Ordered)
            {
                // If Patrol Type Random, Set a random patrol destination from patrol markers to the goal destination
                if(patrolType == PatrolType.Random) patrol_index = IncrementIndex(patrol_index, patrol_markers, false, true); 
                
                curPath = CreatePath(); // Create a new path in either case
                
                // If Patrol Type Ordered, increment the index so that the next destination is set for the next SetNextPath call
                if(patrolType == PatrolType.Ordered)patrol_index = IncrementIndex(patrol_index, patrol_markers); // Increment patrol index, loop back to 0 when list size is reached
            }
            else {  /*? Free Patrol implementation here*/ }
            enemy.movement.SetTarget(curPath.Pop()); // Pop the first target position from the path stack
        }

        
        // HEADER: Enemy Detection

        private void ChaseTarget(Transform target)
        {
            enemy.SetTarget(target);
            enemy.ChangeToState(NPCStateEnum.Chasing);
        }
        
        private void FindPrimaryTarget()
        {
            var targets = GetFriendliesInView();
            if (targets.Count <= 0) return;
            var target = targets[0];
            float min_distance = int.MaxValue;
            foreach (var t in targets)
            {
                var dist = Vector3.Distance(position, t.position);
                if (!(dist < min_distance)) continue;
                min_distance = dist;
                target = t;
            }

            if (target) { ChaseTarget(target); }
        }

        private List<Transform> GetFriendliesInView()
        {
            var friendlies_list = GlobalGameManager.GetFriendlies();
            var friendlies_in_front = friendlies_list.Where(friendly => enemy.movement.InFront(friendly.position));
            var friendlies_in_view = friendlies_in_front.Where(friendly =>
            {
                Physics.Raycast(position, enemy.movement.GetDirection(friendly.position), out var hit, enemy.detection.GetValue());
                return hit.transform && (hit.transform.CompareTag("Friendly") || hit.transform.CompareTag("Player"));
            }).ToList();
            return friendlies_in_view;
        }
        
        
        // HEADER: HELPER METHODS
        // HDESC: Just some methods to help make the code more readable
        
        /// <summary> Returns true if the enemy doesn't have a path / curPath is empty </summary>
        private bool NoPath() { return curPath.Count == 0; }

        /// <summary> Use A* search to create a path to the enemy's next destinationn</summary>
        private Stack<Vector3> CreatePath() {  return AStarSearch.Search(enemy.transform, (GetDestination())); }

        /// <summary> Return the vector coordinates of the enemy's current target destination </summary>
        private Vector3 GetDestination() { return patrol_markers[patrol_index].position; }

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
