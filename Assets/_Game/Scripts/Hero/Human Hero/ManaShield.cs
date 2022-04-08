using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaShield : MonoBehaviour, IDamageable
{
    private HumanHero _hero;
    private void Start()
    {
        _hero = transform.parent.GetComponent<HumanHero>();
        _hero._manaShieldEnabled = true;
    }
    
    public void TakeDamage(float damage, GameObject damager)
    {
        _hero.mana -= damage;

        if (_hero.mana <= 0)
        {
            _hero.mana = 0;
            _hero._manaShieldOnCooldown = true;
            Destroy(gameObject);
        }
    }

    public void OnDeath(GameObject killer)
    {
        throw new NotImplementedException();
    }
}
