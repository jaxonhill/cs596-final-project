using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 1;
    public float attackRange = 1f;
    public float attackCooldown = 2f;
    public LayerMask targetLayer;

    private float nextAttackTime = 0f;

    public void TryAttack()
    {
        if (Time.time < nextAttackTime)
        {
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, attackRange, targetLayer);

        foreach (Collider hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.DamageTaken(attackDamage);
            }
        }

        nextAttackTime = Time.time + attackCooldown;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + transform.forward, attackRange);
    }

    // Update is called once per frame
    void Update()
    {
        if (CompareTag("Player") && Input.GetKeyDown(KeyCode.Space))
        {
            TryAttack();   
        }
    }
}
