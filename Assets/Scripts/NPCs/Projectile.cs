using Components;
using GameManaging;
using NPCs.Enemies;
using TriInspector;
using UnityEngine;

namespace NPCs
{
    [HideMonoScript]
    public class Projectile : MonoBehaviour
    {
        private RangedEnemy enemy;
        /// The last know coordinate of the target before this projectile was fired
        private Vector3 target;

        public void Init(RangedEnemy this_enemy) {
            enemy = this_enemy;
            target = enemy.target.position;
        }
    
        // When instantiated, look at the target 
        private void Start() { transform.LookAt(target); }

        // Move the projectile forward (with the given speed)
        private void Update() { transform.position +=  Time.deltaTime * enemy.projectileSpeed * transform.forward; }

        // When the projectile collides with something
        public void OnTriggerEnter(Collider collision)
        {
            if (!collision.transform) return; // If the collision doesn't have a transform (for some reason), return
            
            var hit = collision.transform;
            
            if (hit.CompareTag(enemy.transform.tag)) return; // If the hit transform was the attacker (or an ally of them), return
            
            // If the hit transform is an enemy of the attacker, deal damage
            var tags = GlobalGameManager.GetTargetTags(enemy.transform);
            if (tags.Contains(hit.tag)) { hit.GetComponent<Health>().OnDamaged(1); }
            
            // Destroy the projectile
            Destroy(gameObject);
        }
    }
}
