using System.Collections.Generic;
using NPCs.Enemies;
using Unity.VisualScripting;
using UnityEngine;

namespace NPCs.States.ChaseStates
{
    public class RangedEnemyChaseState : EnemyChaseState
    {
        /* * * * *
         * CONSTS *
         * * * * */
        /// Used to bypass a return value without committing to the actual truth value (true in this case)
        private const bool DISREGARD = true;
        /// Slack ensures the enemy get 10 units closer to the target than necessary, to ensure they don't immediately fall out of attack state
        private const int HARD_SLACK = 10;
        private const int SOFT_SLACK = 5;
        private const Stack<Vector3> NO_PATH = null;
        
        /* * * * * * * * * 
         * Runtime Fields *
         * * * * * * * * */
        /// The current stack of vectors leading to a destination point
        private Stack<Vector3> curPath;
        /// True if the enemy is already following a path to a point in range of the target
        private bool followingTarget;
        /// The distance the enemy will reach before entering an attack state
        private int Hard_Slack_Distance => attack.GetRange() - HARD_SLACK;
        
        
        // HEADER: CONSTRUCTOR
        
        public RangedEnemyChaseState(RangedEnemy enemy) : base(enemy){}
        
        
        protected override void FollowTarget()
        {
            if (movement.WithinLocation(attack.GetRange() - Hard_Slack_Distance, target.position)) return;
            if (!followingTarget) {
                curPath = AStarSearch.Search(enemy.transform, target.position, Hard_Slack_Distance);
                if (curPath == NO_PATH) return;
                followingTarget = true; 
                movement.SetDestination(curPath.Pop());
            }
            if(!movement.AtDestination()){movement.MoveTowardsDestination();}
            else if (NoPath()) { followingTarget = false; }
            else { movement.SetDestination(curPath.Pop()); }
        }

        protected override bool CheckIfInRange() {
            return movement.WithinLocation(attack.GetRange() - SOFT_SLACK, target.position); }

        protected override bool CheckIfInSight() { // ReSharper disable once ConvertIfStatementToReturnStatement
            if (followingTarget) return DISREGARD; // Don't worry about whether the target is in sight when following them
            return base.CheckIfInSight();
        }

        /// Returns true if the path is null or empty
        private bool NoPath() { return curPath == NO_PATH || curPath.Count == 0; }
    }
}
