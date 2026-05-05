using System.Collections.Generic;
using System.Threading.Tasks;
using Components;
using Components.NPCComponents;
using Cysharp.Threading.Tasks;
using NPCs.Enemies;
using Unity.VisualScripting;
using UnityEngine;

namespace NPCs.States
{
    public class SearchState : NPCState
    {
        /* * * * * * * * * *
         * NPC Components  *
         * * * * * * * * * */
        /// The primary script for enemies: <see cref="BaseEnemy">BaseEnemy</see>
        private readonly BaseEnemy enemy;

        private Vector3 position => enemy.transform.position;
        
        /* * * * * * * * * * *
         * Search Components *
         * * * * * * * * * * */
        /// The entity this enemy is searching for 
        private Transform target => enemy.Target;
        /// The current path from the Start Node to the Goal Node (from Top to Bottom of the stack)
        private Stack<Vector3> curPath;
        /// The second location to search for a lost target 
        private Vector3 secondDestination;
        /// Whether the entity is searching the second search location
        private bool onSecondSearch;
        /// The delay between marking the first and second search locations
        private int searchDelay;
        
        
        // HEADER: CONSTRUCTOR

        public SearchState(BaseEnemy new_enemy) : base(new_enemy)
        {
            enemy = new_enemy;
        }
        
        
        // HEADER: STATE METHODS
        
        // ReSharper disable Unity.PerformanceAnalysis
        // ReSharper disable once AsyncVoidMethod
        public override async UniTask Enter()
        {
            curPath = null;

            pause = true;
            while (curPath.IsUnityNull())
            {
                curPath = AStarSearch.Search(enemy.transform, target.position);
                await UniTask.Delay(10);
            }
            pause = false;
            // Create a path to the last known location of the target
            
            // Start moving the enemy through the path
            if (curPath != null) movement.SetDestination(curPath.Pop());
            // After a delay, mark the second location
            await UniTask.Delay(searchDelay);
            secondDestination = target.position;
        }

        public override async UniTask Run()
        {
            // If the enemy is found, return to a Chase State
            if (TargetInView()) { await stateMachine.ChangeToState(NPCStateEnum.Chasing); }
            
            // If Searh execution is paused, return, otherwise do Search for Target
            if (pause) return;
            await Search();
        }

        public override UniTask Exit() {return UniTask.CompletedTask;}
        
        
        // HEADER: SEARCH METHODS
        
        // ReSharper disable Unity.PerformanceAnalysis
        /// Implementation for making an Enemy follow a given path 
        private async Task Search()
        {
            // If the enemy has not yet reached the target position, move towards it
            if (!movement.AtDestination()) { movement.MoveTowardsDestination(); }
            
            // If the enemy has reached the target position, but the path has not been fully traversed, get the next target position
            else if (!NoPath()){ movement.SetDestination(curPath.Pop()); }
            
            // If the Stack is empty, 
            else {
                if (!onSecondSearch) { // If not yet searching the second location
                    // If (and when) the second location is marked, create a path to it and move the enemy down this path
                    _ = InvokeWithWaitUntil(() => {
                        onSecondSearch = true;
                        curPath = AStarSearch.Search(enemy.transform, secondDestination);
                        movement.SetDestination(curPath.Pop());
                    }, () => !secondDestination.IsUnityNull());
                    return;
                }
                enemy.SetAnimationTrigger("Idle");
                await UniTask.Delay(searchDelay); // Once both search locations have been checked, and target is not found, return to Idle State
                await stateMachine.ChangeToState(NPCStateEnum.Idle);
            }
        }
        
        /// Return true if the target is both in front of the enemy and in sight of the enemy
        private bool TargetInView()
        {
            var target_in_front = movement.InFront(target.position); 
            var target_in_sight = PhyTools.RaycastForTransform(position, movement.GetDirection(target.position),detection.GetActiveValue(), 
                                                                                                                target, Color.red );
            return target_in_front && target_in_sight;
        }
        
        
        // HEADER: HELPER METHODS
        
        /// <summary> Returns true if the enemy doesn't have a path / curPath is empty </summary>
        private bool NoPath() { return curPath.Count == 0; }
    }
}