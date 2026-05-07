using UnityEngine;

namespace Combat
{
    public class Damageable : MonoBehaviour, IDamageable
    {
        [Header("Health")]
        [SerializeField] private int maxHealth = 10;

        [Header("Invincibility")]
        [SerializeField] private float damageInvincibilityDuration = 0.5f;

        private int currentHealth;
        private bool isStateInvincible;
        private float damageInvincibleUntil;

        public bool IsDamageable => currentHealth > 0 && !isStateInvincible && Time.time >= damageInvincibleUntil;
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
            Debug.Log($"[Damageable] {name} initialized: {currentHealth}/{maxHealth} HP", this);
        }

        public void TakeDamage(int amount, GameObject source)
        {
            if (!IsDamageable)
            {
                Debug.Log($"[Damageable] {name} ignored {amount} damage from {source?.name} — invincible until {damageInvincibleUntil:F2}", this);
                return;
            }

            int healthBefore = currentHealth;
            currentHealth -= amount;
            Debug.Log($"[Damageable] {name} took {amount} damage from {source?.name}: {healthBefore} → {currentHealth} HP", this);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Debug.Log($"[Damageable] {name} died from {source?.name}", this);
                // Death handling can be hooked here later
                return;
            }

            BeginTimedInvincibility(damageInvincibilityDuration);
        }

        public void SetStateInvincible(bool isInvincible)
        {
            isStateInvincible = isInvincible;
            Debug.Log($"[Damageable] {name} state invincibility = {isInvincible}", this);
        }

        public void BeginTimedInvincibility(float duration)
        {
            damageInvincibleUntil = Time.time + duration;
            Debug.Log($"[Damageable] {name} timed invincibility started until {damageInvincibleUntil:F2}", this);
        }

        public void Heal(int amount)
        {
            int healthBefore = currentHealth;
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            Debug.Log($"[Damageable] {name} healed: {healthBefore} → {currentHealth} HP", this);
        }
    }
}
