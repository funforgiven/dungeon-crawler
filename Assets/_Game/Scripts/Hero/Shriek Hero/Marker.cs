using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    private ShriekHero _hero;
    private void Start()
    {
        _hero = transform.parent.GetComponent<ShriekHero>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (_hero._sprintActive)
        {
            var enemy = col.GetComponent<Enemy>();
            if(enemy) 
                if(!_hero._markedEnemies.Contains(enemy))
                    _hero._markedEnemies.Add(enemy);
        }
    }
}
