using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    internal float damage;
    internal GameObject owner;
    
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        ApplyDamage(col.GetComponent<IDamageable>());
        Destroy(gameObject);
    }

    void ApplyDamage(IDamageable damageable)
    {
        damageable.TakeDamage(damage, owner);
    }
}
