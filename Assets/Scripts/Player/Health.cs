using UnityEngine;

public class Health : MonoBehaviour
{
    public int totalHealth = 3;
    public int currentHealth;

    void Start()
    {
        currentHealth = totalHealth;
    }

    public void DamageTaken(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (CompareTag("Player"))
        {
            Debug.Log("Player died");
            return;
        }

        Destroy(gameObject);
    }
}
