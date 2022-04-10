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

    internal Enemy owner;
    [HideInInspector] public Transform target;

    private Animator _animator;
    private bool _explode = false;
    private bool _startedMoving = false;

    private float _chargeTimeElapsed = 0;


    private void Start()
    {
        _animator = GetComponent<Animator>();
        transform.SetParent(owner.transform);
    }
    private void Update()
    {
        if (!owner) Destroy(gameObject);
        if (owner._inCC && !_startedMoving) Destroy(gameObject);

        if (!_startedMoving)
        {
            if(!owner._inCC)
                owner._agent.ResetPath();
        }
        
        _chargeTimeElapsed += Time.deltaTime;
        if (_chargeTimeElapsed > chargeTime && !_startedMoving)
        {
            _animator.SetBool("Move", true);
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
        if (damageable == null) return;
        damageable.TakeDamage(damageToDeal, owner.gameObject, DamageType.Magical);
    }
}
