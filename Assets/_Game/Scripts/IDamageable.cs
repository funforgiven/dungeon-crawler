using UnityEngine;

public interface IDamageable
{
    abstract void TakeDamage(float damage, GameObject damager, DamageType damageType);
    abstract void OnDeath(GameObject killer);
}

public enum DamageType
{
    Physical,
    Magical,
    Fire
}