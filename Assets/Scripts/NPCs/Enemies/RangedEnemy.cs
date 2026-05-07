using NPCs.States.AttackStates;
using NPCs.States.ChaseStates;
using TriInspector;
using UnityEngine;

namespace NPCs.Enemies
{
    /// <summary> Class used to define and instantiate Melee Enemies </summary>
    [HideMonoScript]
    public class RangedEnemy : BaseEnemy
    {
        [Tooltip("Prefab of the projectile object"), SerializeField]
        private GameObject projectile;
        public GameObject Projectile => projectile;
        
        [Tooltip("Offset from the NPC where an arrow should spawn")]
        public int offsetAmount { get; private set; }
        
        [Tooltip("How fast the projectile should travel")]
        public int projectileSpeed { get; private set; }
        
        
        public RangedEnemy() { willSearch = false; }
    }
}
