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
        
        if(_animator) _animator.SetBool("Move", true);
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.GetComponent<IDamageable>() != null)
            ApplyDamage(col.GetComponent<IDamageable>());

        if (_animator)
        {
            _animator.SetBool("Explode", true);
            DestroyCouroutine(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyCouroutine(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        Destroy(gameObject);
    }
        
    void ApplyDamage(IDamageable damageable)
    {
        damageable.TakeDamage(damage, owner);
    }
}
