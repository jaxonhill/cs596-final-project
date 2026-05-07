using TriInspector;
using UnityEngine;

namespace NPCs.Enemies
{
    /// <summary> Class used to define and instantiate Melee Enemies </summary>
    [HideMonoScript]
    public class RangedEnemy : BaseEnemy
    {
        [Title("Projectiles")]
        
        [Tooltip("Prefab of the projectile object"), SerializeField]
        private GameObject projectile;
        public GameObject Projectile => projectile;

        [Tooltip("Offset from the NPC where an arrow should spawn"), SerializeField]
        private Vector3 offsetAmount;
        public Vector3 OffsetAmount => offsetAmount;

        [Tooltip("How fast the projectile should travel"), SerializeField]
        private int projectileSpeed;
        public int ProjectileSpeed => projectileSpeed;

        public bool nowThrow;


        public RangedEnemy() { willSearch = false; }
    }
}
