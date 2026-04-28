using UnityEngine;

public class SwordAttackHitbox : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.1f;

    public GameObject Owner { get; private set; }
    public int Damage { get; private set; }
    public LayerMask TargetLayers { get; private set; }

    public void Initialize(GameObject owner, int damage, LayerMask targetLayers)
    {
        Owner = owner;
        Damage = damage;
        TargetLayers = targetLayers;
        Destroy(gameObject, lifetime);
    }

    private void Start()
    {
        if (Owner == null)
        {
            Destroy(gameObject, lifetime);
        }
    }
}
