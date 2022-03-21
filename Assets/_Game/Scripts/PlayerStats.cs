using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100;
    private float _health;
    
    // Start is called before the first frame update
    void Start()
    {
        _health = maxHealth;
    }

    // Update is called once per frame
    public void TakeDamage(float damage)
    {
        _health -= damage;
        
        if(_health <= 0)
            Destroy(gameObject);
    }
}
