using System;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace NPCs.Enemies
{
    [Serializable]
    public class PatrolObject 
    {
        // Method specifically used for the Dropdown attribute of patrolType (Creates dropdown list functionality in the inspector)
        private IEnumerable<TriDropdownItem<int>> GetPatrolType()
        {
            return new TriDropdownList<int>
            {
                {"Random", 0},
                {"Ordered", 1},
                {"Free", 2},
            };
        }
        
        [Tooltip("The patrolling type the enemy uses: Ordered, Random, or Free. See PatrolType enum in PatrolState")]
        [Title("Patrolling")] [Dropdown(nameof(GetPatrolType)), SerializeField]
        public int patrolType;
        
        [Tooltip("The positions that enemies will move to when not chasing the player."), SerializeField]
        public List<Transform> patrol_markers;
        
    }
}
