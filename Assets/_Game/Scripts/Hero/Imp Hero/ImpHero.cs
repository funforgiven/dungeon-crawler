using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpHero : Hero
{
    [Header("Fireball")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballCooldown = 10f;
    [SerializeField] private float fireballSpeed = 3f;
    [SerializeField] private float fireballDamage= 3f;
    private GameObject _fireball;
    private bool _fireballOnCooldown = false;
    private float _fireballCurrentCooldown = 0f;

    [Header("Flame Barrier")] 
    [SerializeField] private float flameBarrierCooldown;
    [SerializeField] private int flameBarrierCharge = 3;
    [SerializeField] private float flameBarrierRegeneration = 2f;
    [SerializeField] private float flameBarrierDamage = 25f;
    [SerializeField] private float flameBarrierDamageRadius = 6f;
    [SerializeField] private LayerMask flameBarrierDamageLayer;
    private int _flameBarrierCurrentCharge;
    private bool _flameBarrierActive;
    private bool _flameBarrierOnCooldown = false;
    private float _flameBarrierCurrentCooldown = 0f;


    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        
        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (!_fireballOnCooldown)
            {
                _fireballOnCooldown = true;
                
                _fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);

                var direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                _fireball.GetComponent<Rigidbody2D>().velocity = direction.normalized * fireballSpeed;
                _fireball.GetComponent<Projectile>().owner = gameObject;
                _fireball.GetComponent<Projectile>().damage = fireballDamage;
            }
        }

        if (_fireballOnCooldown)
        {
            _fireballCurrentCooldown += Time.deltaTime;

            if (_fireballCurrentCooldown > fireballCooldown)
            {
                _fireballCurrentCooldown = 0;
                _fireballOnCooldown = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            if (!_flameBarrierOnCooldown)
            {
                _flameBarrierActive = true;
                _flameBarrierOnCooldown = true;
                _flameBarrierCurrentCharge = flameBarrierCharge;
            }
        }
        
        if (_flameBarrierOnCooldown)
        {
            _flameBarrierCurrentCooldown += Time.deltaTime;

            if (_flameBarrierCurrentCooldown > flameBarrierCooldown)
            {
                _flameBarrierCurrentCooldown = 0;
                _flameBarrierOnCooldown = false;
            }
        }

        if (_flameBarrierActive)
        {
            _health += Time.deltaTime * flameBarrierRegeneration;
        }
    }
    
    public override void TakeDamage(float damage, GameObject damager)
    {
        if (_flameBarrierActive)
        {
            _flameBarrierCurrentCharge -= 1;
            if (_flameBarrierCurrentCharge == 0)
                _flameBarrierActive = false;
        
            Explode();
        }

       
        base.TakeDamage(damage, damager);
    }

    private void Explode()
    {
        var overlap = Physics2D.OverlapCircleAll(transform.position, flameBarrierDamageRadius, flameBarrierDamageLayer);

        foreach (var col in overlap)
        {
            var damageable = col.GetComponent<IDamageable>();
            if(damageable != null) damageable.TakeDamage(flameBarrierDamage, gameObject);
        }
    }
}
