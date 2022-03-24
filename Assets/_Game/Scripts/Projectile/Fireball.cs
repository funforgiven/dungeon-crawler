using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] private float damage;
    [SerializeField] private float explosionDamage;

    [HideInInspector] public GameObject owner;

    private Animator _animator;
    private bool _explode = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!_explode)
        {
            _explode = true;
            _animator.SetBool("Explode", _explode);
            ApplyDamage(damage,col.GetComponent<IDamageable>());
            ApplyDamage(explosionDamage,col.GetComponent<IDamageable>());
            return;
        }
  
        ApplyDamage(explosionDamage, col.GetComponent<IDamageable>());
    }

    public void Move(Vector3 startPosition, Vector3 endPosition)
    {
        transform.position = startPosition;
        var direction = endPosition - startPosition;
        GetComponent<Rigidbody2D>().velocity = direction.normalized * speed;
    }

    void ApplyDamage(float damageToDeal, IDamageable damageable)
    {
        damageable.TakeDamage(damageToDeal, owner);
    }
}
