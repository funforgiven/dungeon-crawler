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
    }
}
