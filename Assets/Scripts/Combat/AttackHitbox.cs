using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class AttackHitbox : MonoBehaviour
    {
        [SerializeField] private int damage;
        public int Damage => damage;
        [SerializeField] private LayerMask targetLayers;

        private GameObject owner;
        private HashSet<GameObject> alreadyHitTargets = new HashSet<GameObject>();

        public void Initialize(GameObject newOwner, int newDamage, LayerMask newTargetLayers)
        {
            owner = newOwner;
            damage = newDamage;
            targetLayers = newTargetLayers;
            alreadyHitTargets.Clear();
            Debug.Log($"[AttackHitbox] Initialized on {gameObject.name}: owner={owner?.name}, damage={damage}, layers={targetLayers}", this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (owner != null && other.gameObject == owner)
            {
                return;
            }

            if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            {
                return;
            }

            if (alreadyHitTargets.Contains(other.gameObject))
            {
                Debug.Log($"[AttackHitbox] Blocked damage to {other.name} (already hit)", this);
                return;
            }

            alreadyHitTargets.Add(other.gameObject);
            Debug.Log($"[AttackHitbox] Hit {other.name} on layer {other.gameObject.layer}", this);

            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable == null)
            {
                damageable = other.GetComponentInParent<IDamageable>();
            }

            if (damageable == null)
            {
                Debug.Log($"[AttackHitbox] {other.name} has no IDamageable component", this);
                return;
            }

            if (!damageable.IsDamageable)
            {
                Debug.Log($"[AttackHitbox] Blocked damage to {other.name} (invincible)", this);
                return;
            }

            damageable.TakeDamage(damage, owner);
            Debug.Log($"[AttackHitbox] Dealt {damage} to {other.name}", this);
        }
    }
}
