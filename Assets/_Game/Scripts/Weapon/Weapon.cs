using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected float damage = 10f;
    
    public Hero owner;
    public abstract void Attack();

    protected void ApplyDamage(Enemy enemy)
    {
        int roll = Random.Range(0, 100);
        if (roll < owner.critRate)
        {
            enemy.TakeDamage(damage * (owner.critDamage/100), owner.gameObject);
        }
        else
            enemy.TakeDamage(damage, owner.gameObject);
    }

    public void Enable()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public void Disable()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
