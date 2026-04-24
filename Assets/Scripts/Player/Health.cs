using UnityEngine;

public class Health : MonoBehaviour
{
    public int totalHealth = 3;
    public int currentHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = totalHealth;
    }

    public void DamageTaken(int damage)
    {
        currentHealth  = currentHealth - damage;
        Debug.Log(gameObject.name + " Health: "  + currentHealth);

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
        }
        else {
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
