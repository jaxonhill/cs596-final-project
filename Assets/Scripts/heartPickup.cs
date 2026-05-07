using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    public int healAmount = 1;
    [SerializeField] private AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Play sound instantly (independent of object)
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // Destroy immediately
        Destroy(gameObject);
    }
}