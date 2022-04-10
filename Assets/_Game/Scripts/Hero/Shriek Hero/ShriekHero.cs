using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

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
    private float _sprintCurrentDuration = 0f;

    [Header("AoE")] 
    [SerializeField] private float aoeDamage = 10f;
    [SerializeField] private float aoeDamageRadius = 3f;
    [SerializeField] private float aoeCooldown = 5f;
    [SerializeField] private LayerMask aoeDamageLayer;
    private bool _aoeOnCooldown = false;
    private float _aoeCurrentCooldown = 0f;
    private float _aoeCurrentDuration = 0f;
    
    [Header("Fear")]
    [SerializeField] private float fearCooldown = 5f;
    [SerializeField] private float fearDuration = 2f;
    [SerializeField] private float fearMaxRange = 10f;
    private bool _fearOnCooldown = false;
    private float _fearCurrentCooldown = 0f;
    private float _fearCurrentDuration = 0f;


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

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (!_aoeOnCooldown)
            {
                _aoeOnCooldown = true;
                
                var overlap = Physics2D.OverlapCircleAll(transform.position, aoeDamageRadius, aoeDamageLayer);

                foreach (var col in overlap)
                {
                    var damageable = col.GetComponent<IDamageable>();
                    if(damageable != null) damageable.TakeDamage(aoeDamage, gameObject, DamageType.Magical);
                }
            }
        }

        if (_aoeOnCooldown)
        {
            _aoeCurrentCooldown += Time.deltaTime;

            if (_aoeCurrentCooldown > aoeCooldown)
            {
                _aoeCurrentCooldown = 0;
                _aoeOnCooldown = false;
            }
        }
        
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!_fearOnCooldown)
            {
                if (_markedEnemies.Count > 0)
                {
                    foreach (var enemy in _markedEnemies)
                    {
                        StartCoroutine(Fear(enemy));
                    }

                    _markedEnemies = new List<Enemy>();
                }
            }
        }
        
        if (_fearOnCooldown)
        {
            _fearCurrentCooldown += Time.deltaTime;

            if (_fearCurrentCooldown > fearCooldown)
            {
                _fearCurrentCooldown = 0;
                _fearOnCooldown = false;
            }
        }
    }

    private IEnumerator Fear(Enemy enemy)
    {
        var pos = enemy.transform.position;
        var direction =  pos + pos - transform.position;
        Vector3 runPosition = direction.normalized * fearMaxRange;

        NavMeshHit hit;
        NavMesh.SamplePosition(runPosition, out hit, fearMaxRange, 1);
        
        enemy._inCC = true;
        enemy._agent.SetDestination(runPosition);
        
        yield return new WaitForSeconds(fearDuration);
        enemy._inCC = false;
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
