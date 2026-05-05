using System.Collections.Generic;
using NPCs.Enemies;
using Unity.VisualScripting;
using UnityEngine;

namespace NPCs.States.ChaseStates
{
    public class RangedEnemyChaseState : EnemyChaseState
    {
        private bool followingTarget;

        private Stack<Vector3> curPath;

        private const bool DISREGARD = true; 
        
        public RangedEnemyChaseState(RangedEnemy enemy) : base(enemy){}

        protected override void FollowTarget()
        {
            if (movement.WithinLocation(attack.GetRange() - 10, target.position)) return;
            if (!followingTarget)
            {
                curPath = null;
                followingTarget = true;
                while (curPath == null)
                {
                    curPath = AStarSearch.Search(enemy.transform, target.position, attack.GetRange() - 10);
                }
                movement.SetDestination(curPath.Pop());
            }
            if(!movement.AtDestination()){movement.MoveTowardsDestination();}
            else if (curPath.Count == 0) { followingTarget = false; }
            else { movement.SetDestination(curPath.Pop()); }
        }

        protected override bool CheckIfInRange()
        {
            return movement.WithinLocation(attack.GetRange() - 5, target.position);
        }

        protected override bool CheckIfInSight()
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (followingTarget) return DISREGARD;
            return base.CheckIfInSight();
        }
    }
}
