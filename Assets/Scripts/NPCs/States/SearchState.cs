using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NPCs.Enemies;
using Unity.VisualScripting;
using UnityEngine;

namespace NPCs.States
{
    public class SearchState : NPCState
    {
        /* * * * * * * * * *
         * Search Components  *
         * * * * * * * * * */
        private Transform target => enemy.GetTarget();
        /// <summary> The current path from the Start Node to the Goal Node (from Top to Bottom of the stack)</summary>
        private Stack<Vector3> curPath;

        private Vector3 secondTarget;
        
        /* * * * * * * * * *
         * NPC Components  *
         * * * * * * * * * */
        private readonly BaseEnemy enemy;

        private bool secondSearch = false;
        private bool startSearch = true;
        
        public SearchState(BaseEnemy new_enemy) { enemy = new_enemy; }
        
        public override async void Enter()
        {
            curPath = AStarSearch.Search(enemy.transform, target.position);
            enemy.SetTargetPos(curPath.Pop());
            await UniTask.Delay(1500);
            secondTarget = target.position;
        }

        public override async UniTask Run()
        {
            if (TargetInView())
            {
                enemy.ChangeToState(NPCStateEnum.Chasing);
            }
            if (!startSearch) return;
            await Search();
        }

        public override void Exit()
        {
            
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary> Implementation for making an Enemy follow a given path </summary>
        private async Task Search()
        {
            // If the enemy has not yet reached the target position, move towards it
            if (!enemy.AtDestination()) { enemy.MoveTowardsDestination(); }
            
            // If the enemy has reached the target position, but the path has not been fully traversed, get the next target position
            else if (!NoPath()){ enemy.SetTargetPos(curPath.Pop()); /* Pop the next target position from the current path */ }
            
            // If the Stack is empty, 
            else
            {
                if (!secondSearch)
                {
                    startSearch = false;
                    await UniTask.WaitUntil(() => !secondTarget.IsUnityNull());
                    secondSearch = true;
                    curPath = AStarSearch.Search(enemy.transform, secondTarget);
                    enemy.SetTargetPos(curPath.Pop());
                    startSearch = true;
                    return;
                }
                await UniTask.Delay(1000);
                enemy.ChangeToState(NPCStateEnum.Idle);
            }
        }
        
        /// <summary> Returns true if the enemy doesn't have a path / curPath is empty </summary>
        private bool NoPath() { return curPath.Count == 0; }
        
        private bool TargetInView()
        {
            var target_in_front = enemy.InFront(target.position);
            Physics.Raycast(enemy.GetPosition(), enemy.GetDirection(target.position), out var hit, enemy.GetDetectionRange());
            return target_in_front && hit.transform && hit.transform == target;
        }
    }
}