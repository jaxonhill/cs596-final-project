using UnityEngine;

public interface IDamageable
{
    bool IsDamageable { get; }
    void TakeDamage(int amount, GameObject source);
}
