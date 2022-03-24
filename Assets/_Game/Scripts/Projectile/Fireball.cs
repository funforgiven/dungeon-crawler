using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] private float damage;
    [SerializeField] private float explosionDamage;
    [SerializeField] private float chargeTime;

    [HideInInspector] public GameObject owner;
    [HideInInspector] public Transform target;

    private Animator _animator;
    private bool _explode = false;
    private bool _initialized = false;
    private bool _startedMoving = false;

    private float _chargeTimeElapsed = 0;


    private void Update()
    {
        if (!owner) return;
        
        if (!_initialized)
        {
            _animator = GetComponent<Animator>();
            owner.GetComponent<Enemy>()._agent.enabled = false;
            _initialized = true;
        }

        _chargeTimeElapsed += Time.deltaTime;

        if (_chargeTimeElapsed > chargeTime && !_startedMoving)
        {
            _animator.SetBool("Move", true);
            owner.GetComponent<Enemy>()._agent.enabled = true;
            Move(transform.position, target.position);
            _startedMoving = true;
        }
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
        var direction = endPosition - startPosition;
        GetComponent<Rigidbody2D>().velocity = direction.normalized * speed;
    }

    void ApplyDamage(float damageToDeal, IDamageable damageable)
    {
        damageable.TakeDamage(damageToDeal, owner);
    }
}
