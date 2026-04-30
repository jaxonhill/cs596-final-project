using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerMotor motor;
    [SerializeField] private GameObject swordAttackHitboxPrefab;
    [SerializeField] private int swordAttackDamage = 1;
    [SerializeField] private float swordAttackDuration = 0.50f;
    [SerializeField] private float swordAttackHitboxDelay = 0.25f;
    [SerializeField] private Vector3 swordAttackHitboxLocalOffset = new Vector3(0f, 1f, 1f);

    private GameObject spawnedHitbox;
    private bool hasSwordAttackHitboxSpawned;
    private float swordAttackHitboxSpawnTime;
    private float swordAttackHitboxEndTime;

    public bool IsSwordAttackFinished => Time.time >= swordAttackHitboxEndTime;

    private void Awake()
    {
        if (motor == null)
        {
            Debug.LogError($"PlayerCombat requires PlayerMotor assigned to '{nameof(motor)}' on {name}.", this);
        }
        else
        {
            Debug.Log($"PlayerCombat found required PlayerMotor reference on {name}.", this);
        }

        if (swordAttackHitboxPrefab == null)
        {
            Debug.LogError($"PlayerCombat requires sword attack hitbox prefab assigned to '{nameof(swordAttackHitboxPrefab)}' on {name}.", this);
        }
        else
        {
            Debug.Log($"PlayerCombat found required sword attack hitbox prefab reference on {name}.", this);
        }

        if (motor == null || swordAttackHitboxPrefab == null)
        {
            enabled = false;
        }
    }

    public void BeginSwordAttack()
    {
        if (motor == null)
        {
            Debug.LogError($"PlayerCombat cannot begin sword attack because '{nameof(motor)}' is missing on {name}.", this);
            return;
        }

        swordAttackHitboxEndTime = Time.time + swordAttackDuration;
        swordAttackHitboxSpawnTime = Time.time + swordAttackHitboxDelay;
        hasSwordAttackHitboxSpawned = false;
        motor.StopHorizontalMovement();
    }

    public void UpdateSwordAttack()
    {
        if (swordAttackHitboxPrefab == null)
        {
            Debug.LogError($"PlayerCombat cannot update sword attack because '{nameof(swordAttackHitboxPrefab)}' is missing on {name}.", this);
            return;
        }

        if (hasSwordAttackHitboxSpawned)
        {
            if (Time.time >= swordAttackHitboxSpawnTime) {
                SpawnSwordAttackHitbox();
            }
            if (Time.time >= swordAttackHitboxEndTime) {
                Destroy(spawnedHitbox);
            }
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
    }
}
