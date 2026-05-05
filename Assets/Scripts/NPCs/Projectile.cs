using Components;
using NPCs.Enemies;
using TriInspector;
using UnityEngine;

namespace NPCs
{
    [HideMonoScript]
    public class Projectile : MonoBehaviour
    {
        private RangedEnemy enemy;
        private Vector3 target;

        public void Init(RangedEnemy this_enemy)
        {
            enemy = this_enemy;
            target = enemy.Target.position;
        }
    
        private void Start()
        {
            transform.LookAt(target);
        }

        // Update is called once per frame
        private void Update() { transform.position +=  Time.deltaTime * enemy.ProjectileSpeed * transform.forward; }

        public void OnTriggerEnter(Collider collision)
        {
            if (!collision.transform) return;
            var hit = collision.transform;
            if (hit.CompareTag(enemy.transform.tag)) return;
            var tags = GlobalGameManager.GetTargetTags(enemy.transform);
            
            if (tags.Contains(hit.tag)) { hit.GetComponent<Health>().OnDamaged(1); }
            
            Destroy(gameObject);
        }
    }
}
