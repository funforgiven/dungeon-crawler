using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShriekHero : Hero
{
    [Header("Mark")]
    internal List<Enemy> _markedEnemies = new List<Enemy>();
    
    [Header("Sprint")]
    [SerializeField] private float sprintSpeed = 1.5f;
    [SerializeField] private float sprintDuration = 4f;
    [SerializeField] private float sprintCooldown = 3f;
    internal bool _sprintActive = false;
    private bool _sprintOnCooldown = false;
    private float _sprintCurrentCooldown = 0f;
    private float _sprintCurrentDuration;

    [Header("Curse")] 
    [SerializeField] private float curseDamage = 3f;


    protected override void Update()
    {
        base.Update();
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            sword.Attack("Mark");
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!_sprintOnCooldown)
            {
                _sprintOnCooldown = true;
                _sprintActive = true;
                _sprintCurrentDuration = 0;
                
                walkSpeed = defaultWalkSpeed * sprintSpeed;
                Physics2D.IgnoreLayerCollision(6, 7, true);
            }
        }

        if (_sprintActive)
        {
            _sprintCurrentDuration += Time.deltaTime;
            if (_sprintCurrentDuration > sprintDuration)
            {
                _sprintActive = false;
                
                walkSpeed = defaultWalkSpeed;
                Physics2D.IgnoreLayerCollision(6, 7, false);
            }
        }
            
        
        if (_sprintOnCooldown)
        {
            _sprintCurrentCooldown += Time.deltaTime;

            if (_sprintCurrentCooldown > sprintCooldown)
            {
                _sprintCurrentCooldown = 0;
                _sprintOnCooldown = false;
            }
        }
    }

    public override void ApplyDamage(Enemy enemy, string identifier)
    {
        base.ApplyDamage(enemy, identifier);
        
        if (identifier == "Mark")
        {
            if(!_markedEnemies.Contains(enemy))
                _markedEnemies.Add(enemy);
        }
    }
}
