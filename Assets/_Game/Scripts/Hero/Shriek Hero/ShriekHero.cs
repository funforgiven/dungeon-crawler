using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class ShriekHero : Hero
{
    internal List<Enemy> _markedEnemies = new List<Enemy>();
    
    [Header("Sprint")]
    [SerializeField] private float sprintSpeed = 1.5f;
    [SerializeField] private float sprintDuration = 4f;
    [SerializeField] private float sprintCooldown = 3f;
    [SerializeField] [Range(0, 1)] private float sprintOpacity = 0.5f;
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

    [Header("Fear")]
    [SerializeField] private float fearCooldown = 5f;
    [SerializeField] private float fearDuration = 2f;
    [SerializeField] private float fearMaxRange = 10f;
    private bool _fearOnCooldown = false;
    private float _fearCurrentCooldown = 0f;

    [Header("Curse")] 
    [SerializeField] private float curseDamage = 5f;
    [SerializeField] private float curseCooldown = 5f;
    [SerializeField] private float curseDuration = 2f;
    private bool _curseActive = false;
    private bool _curseOnCooldown = false;
    private float _curseCurrentCooldown = 0f;
    private float _curseCurrentDuration = 0f;
    internal List<Enemy> _cursedEnemies = new List<Enemy>();


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
                var col = _spriteRenderer.color;
                _spriteRenderer.color = new Color(col.r, col.b, col.g, sprintOpacity);
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
                var col = _spriteRenderer.color;
                _spriteRenderer.color = new Color(col.r, col.b, col.g, 1);
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
                    _fearOnCooldown = true;
                    
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
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!_curseOnCooldown)
            {
                _curseOnCooldown = true;
                _curseActive = true;
                _curseCurrentDuration = 0;
                
                _cursedEnemies.AddRange(_markedEnemies);
                _markedEnemies = new List<Enemy>();
            }
        }
        
        if (_curseActive)
        {
            _curseCurrentDuration += Time.deltaTime;
            if (_curseCurrentDuration > curseDuration)
            {
                _curseActive = false;
                _cursedEnemies = new List<Enemy>();
            }
        }
        
        if (_curseOnCooldown)
        {
            _curseCurrentCooldown += Time.deltaTime;

            if (_curseCurrentCooldown > curseCooldown)
            {
                _curseCurrentCooldown = 0;
                _curseOnCooldown = false;
            }
        }
    }

    private IEnumerator Fear(Enemy enemy)
    {
        var pos = enemy.transform.position;
        var direction =  pos - transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(direction, out hit, fearMaxRange, 1);
        
        enemy._inCC = true;
        enemy._agent.SetDestination(hit.position);
        
        yield return new WaitForSeconds(fearDuration);
        enemy._inCC = false;
    }
    public override void ApplyDamage(Enemy enemy, string identifier, float damage = -1, DamageType damageType = DamageType.Physical)
    {
        if (_curseActive && _cursedEnemies.Contains(enemy))
            base.ApplyDamage(enemy, identifier, swordDamage + curseDamage);
        else
            base.ApplyDamage(enemy, identifier, damage, damageType);
        
        
        if (identifier == "Mark")
        {
            if(!_markedEnemies.Contains(enemy))
                _markedEnemies.Add(enemy);
        }
    }
}
