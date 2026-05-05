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
        [SerializeField, Tooltip("Prefab of the projectile object")]
        private GameObject projectile;
        
        [SerializeField, Tooltip("Offset from the NPC where an arrow should spawn")]
        private int offsetAmount = 1;
        
        [SerializeField, Tooltip("How fast the projectile should travel")]
        private int projectileSpeed = 3;
        
        
        public RangedEnemy()
        {
            willSearch = false;
        }

        public GameObject Projectile => projectile;
        
        public int SpawnOffset => offsetAmount;
        
        public int ProjectileSpeed => projectileSpeed;
    }
}
