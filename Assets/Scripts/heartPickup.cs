using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    public int healAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Later, your teammate's health script can have a Heal(int amount) method.
        var health = other.GetComponent("Health");

        if (health != null)
        {
            health.SendMessage("Heal", healAmount, SendMessageOptions.DontRequireReceiver);
        }

        Destroy(gameObject);
    }
}