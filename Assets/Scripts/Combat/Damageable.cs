using UnityEngine;

namespace Combat
{
    public class Damageable : MonoBehaviour, IDamageable
    {
        [Header("Health")]
        [SerializeField] private int maxHealth = 10;

        [Header("Invincibility")]
        [SerializeField] private float damageInvincibilityDuration = 0.5f;

        [SerializeField] private int currentHealth;
        private bool isStateInvincible;
        private float damageInvincibleUntil;

        public bool IsDamageable => currentHealth > 0 && !isStateInvincible && Time.time >= damageInvincibleUntil;
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int amount, GameObject source)
        {
            if (!IsDamageable) return;

            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                return;
            }

            BeginTimedInvincibility(damageInvincibilityDuration);
        }

        public void BeginTimedInvincibility(float duration)
        {
            damageInvincibleUntil = Time.time + duration;
        }

        // ✅ REQUIRED by your roll system
        public void SetStateInvincible(bool isInvincible)
        {
            isStateInvincible = isInvincible;
        }

        // ✅ Healing for heart pickups
        public void Heal(int amount)
        {
            if (amount <= 0) return;

            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        }
    }
}