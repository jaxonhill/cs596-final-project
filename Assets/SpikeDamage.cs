using UnityEngine;
using Combat;

public class SpikeDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Damageable damageable = other.GetComponentInParent<Damageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount, gameObject);
        }
    }
}