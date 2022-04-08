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
        _animator.SetBool("Move", true);
    }

  /*  private void OnTriggerEnter2D(Collider2D col)
    {
        ApplyDamage(col.GetComponent<IDamageable>());
        _animator.SetBool("Explode", _explode);
        DestroyCouroutine()
    }

    private IEnumerator DestroyCouroutine(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

        Destroy(gameObject);
    }

    void ApplyDamage(IDamageable damageable)
    {
        damageable.TakeDamage(damage, owner);
    }*/
}
