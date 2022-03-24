using UnityEngine;

public interface IDamageable
{
    abstract void TakeDamage(float damage, GameObject damager);
    abstract void OnDeath(GameObject killer);
}