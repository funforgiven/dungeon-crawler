using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] private float damage;
    private void OnTriggerEnter2D(Collider2D col)
    {
        var hero = col.GetComponent<Hero>();
        if (hero)
        {
            hero.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
