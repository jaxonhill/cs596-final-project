using Combat;
using UnityEngine;

namespace Player {
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] private GameObject swordAttackHitboxPrefab;
        [SerializeField] private int swordAttackDamage = 1;
        [SerializeField] private float swordAttackDuration = 0.50f;
        [SerializeField] private float swordAttackHitboxDelay = 0.25f;
        [SerializeField] private Vector3 swordAttackHitboxLocalOffset = new Vector3(0f, 1f, 1f);
        [SerializeField] private LayerMask attackTargetLayers;

        private GameObject spawnedHitbox;
        private bool hasSwordAttackHitboxSpawned;
        private float swordAttackHitboxSpawnTime;
        private float swordAttackHitboxEndTime;

        public bool IsSwordAttackFinished => Time.time >= swordAttackHitboxEndTime;

        private void Awake()
        {
            if (swordAttackHitboxPrefab == null)
            {
                Debug.LogError($"PlayerCombat requires sword attack hitbox prefab assigned to '{nameof(swordAttackHitboxPrefab)}' on {name}.", this);
            }
            else
            {
                Debug.Log($"PlayerCombat found required sword attack hitbox prefab reference on {name}.", this);
            }

            if (swordAttackHitboxPrefab == null)
            {
                enabled = false;
            }
        }

        public void BeginSwordAttack()
        {
            swordAttackHitboxEndTime = Time.time + swordAttackDuration;
            swordAttackHitboxSpawnTime = Time.time + swordAttackHitboxDelay;
            hasSwordAttackHitboxSpawned = false;
        }

        public void UpdateSwordAttack()
        {
            if (swordAttackHitboxPrefab == null)
            {
                Debug.LogError($"PlayerCombat cannot update sword attack because '{nameof(swordAttackHitboxPrefab)}' is missing on {name}.", this);
                return;
            }

            if (!hasSwordAttackHitboxSpawned && Time.time >= swordAttackHitboxSpawnTime)
            {
                SpawnSwordAttackHitbox();
            }

            if (hasSwordAttackHitboxSpawned && Time.time >= swordAttackHitboxEndTime)
            {
                Destroy(spawnedHitbox);
                spawnedHitbox = null;
            }
        }

        private void SpawnSwordAttackHitbox()
        {
            hasSwordAttackHitboxSpawned = true;

            if (swordAttackHitboxPrefab == null)
            {
                Debug.LogWarning("Sword attack hitbox prefab is not assigned.", this);
                return;
            }

            Vector3 spawnPosition = transform.TransformPoint(swordAttackHitboxLocalOffset);
            spawnedHitbox = Instantiate(swordAttackHitboxPrefab, spawnPosition, transform.rotation);

            AttackHitbox attackHitbox = spawnedHitbox.GetComponent<AttackHitbox>();
            if (attackHitbox != null)
            {
                attackHitbox.Initialize(gameObject, swordAttackDamage, attackTargetLayers);
            }
            else
            {
                Debug.LogWarning("[PlayerCombat] Spawned sword hitbox is missing AttackHitbox component.", spawnedHitbox);
            }
        }
    }
}

