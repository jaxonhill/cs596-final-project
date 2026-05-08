using UnityEngine;
using Combat;

public class HeartPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 1;
    [SerializeField] private AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Get player's health
        Damageable damageable = other.GetComponentInParent<Damageable>();

        if (damageable != null)
        {
            damageable.Heal(healAmount);
        }

        // Play sound
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // Destroy heart
        Destroy(gameObject);
    }
}