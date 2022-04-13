using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoblinHero : Hero
{
    [Header("Poison")]
    [SerializeField] private float poisonDamage = 3f;
    [SerializeField] private int poisonDuration = 4;
    [SerializeField] private float poisonInterval = 2f;
    private List<Enemy> _poisonedEnemies = new List<Enemy>();
    
    [Header("Coat")]
    [SerializeField] private float coatCooldown = 12f;
    [SerializeField] private float coatDuration = 5f;
    [SerializeField] private float coatSlowDuration = 3f;
    [SerializeField] private float coatSlowRate = 0.5f;
    private bool _coatActive;
    private bool _coatOnCooldown = false;
    private float _coatCurrentCooldown = 0f;
    private float _coatCurrentDuration = 0f;
    private List<Enemy> _slowedEnemies = new List<Enemy>();
    
    [Header("Sprint")]
    [SerializeField] private float sprintCooldown = 10f;
    [SerializeField] private float sprintSpeed = 1.5f;
    [SerializeField] private float sprintDuration = 4f;
    private bool _sprintActive;
    private bool _sprintOnCooldown = false;
    private float _sprintCurrentCooldown = 0f;
    private float _sprintCurrentDuration = 0f;
    
    [Header("Evasion")]
    [SerializeField] private float evasionCooldown = 10f;
    [SerializeField] private float evasionDuration = 4f;
    [SerializeField] private float evasionDodgeRate = 50f;
    [SerializeField] private float evasionCritRateIncrease = 50f;
    private bool _evasionActive;
    private bool _evasionOnCooldown = false;
    private float _evasionCurrentCooldown = 0f;
    private float _evasionCurrentDuration = 0f;
    
    [Header("Strong Poison")]
    [SerializeField] private float strongPoisonCooldown = 10f;
    [SerializeField] private float strongPoisonDuration = 4f;
    private bool _strongPoisonActive;
    private bool _strongPoisonOnCooldown = false;
    private float _strongPoisonCurrentCooldown = 0f;
    private float _strongPoisonCurrentDuration = 0f;


    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            sword.Attack();
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (!_coatOnCooldown)
            {
                _coatOnCooldown = true;
                _coatActive = true;
                _coatCurrentDuration = 0;
            }
        }
        
        if (_coatActive)
        {
            _coatCurrentDuration += Time.deltaTime;
            if (_coatCurrentDuration > coatDuration)
            {
                _coatActive = false;
            }
        }
        
        if (_coatOnCooldown)
        {
            _coatCurrentCooldown += Time.deltaTime;

            if (_coatCurrentCooldown > coatCooldown)
            {
                _coatCurrentCooldown = 0;
                _coatOnCooldown = false;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!_sprintOnCooldown)
            {
                _sprintOnCooldown = true;
                _sprintActive = true;
                _sprintCurrentDuration = 0;

                walkSpeed = defaultWalkSpeed * sprintSpeed;
            }
        }
        
        if (_sprintActive)
        {
            _sprintCurrentDuration += Time.deltaTime;
            qDuration.fillAmount = (sprintDuration - _sprintCurrentDuration) / sprintDuration;
            if (_sprintCurrentDuration > sprintDuration)
            {
                _sprintActive = false;
                
                walkSpeed = defaultWalkSpeed;
            }
        }
        
        if (_sprintOnCooldown)
        {
            _sprintCurrentCooldown += Time.deltaTime;
            qCooldown.fillAmount = (sprintCooldown - _sprintCurrentCooldown) / sprintCooldown;
            if (_sprintCurrentCooldown > sprintCooldown)
            {
                _sprintCurrentCooldown = 0;
                _sprintOnCooldown = false;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!_evasionOnCooldown)
            {
                _evasionOnCooldown = true;
                _evasionActive = true;
                _evasionCurrentDuration = 0;

                critRate = defaultCritRate + evasionCritRateIncrease;
            }
        }
        
        if (_evasionActive)
        {
            _evasionCurrentDuration += Time.deltaTime;
            if (_evasionCurrentDuration > evasionDuration)
            {
                _evasionActive = false;

                critRate = defaultCritRate;
            }
        }
        
        if (_evasionOnCooldown)
        {
            _evasionCurrentCooldown += Time.deltaTime;

            if (_evasionCurrentCooldown > evasionCooldown)
            {
                _evasionCurrentCooldown = 0;
                _evasionOnCooldown = false;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!_strongPoisonOnCooldown)
            {
                _strongPoisonOnCooldown = true;
                _strongPoisonActive = true;
                _strongPoisonCurrentDuration = 0;
            }
        }
        
        if (_strongPoisonActive)
        {
            _strongPoisonCurrentDuration += Time.deltaTime;
            if (_strongPoisonCurrentDuration > evasionDuration)
            {
                _strongPoisonActive = false;
            }
        }
        
        if (_strongPoisonOnCooldown)
        {
            _strongPoisonCurrentCooldown += Time.deltaTime;

            if (_strongPoisonCurrentCooldown > strongPoisonCooldown)
            {
                _strongPoisonCurrentCooldown = 0;
                _strongPoisonOnCooldown = false;
            }
        }
        
    }

    private IEnumerator Poison(Enemy enemy, float damage, int duration, float cooldown)
    {
        if (_poisonedEnemies.Contains(enemy))
        {
            _poisonedEnemies.Add(enemy);
            yield break;
        }

        _poisonedEnemies.Add(enemy);
        int currentDuration = duration;

        while(currentDuration > 0)
        {
            if (_poisonedEnemies.Count(e => e != null && e.Equals(enemy)) == 2)
            {
                _poisonedEnemies.Remove(enemy);
                currentDuration = duration;
            }

            currentDuration--;
            enemy.TakeDamage(damage, gameObject, DamageType.Fire);
            yield return new WaitForSeconds(cooldown);
        }

        _poisonedEnemies.Remove(enemy);
    }

    private IEnumerator Slow(Enemy enemy, float slowRate, float slowDuration)
    {
        _slowedEnemies.Add(enemy);
        enemy._agent.speed = defaultWalkSpeed * slowRate;
        
        yield return new WaitForSeconds(slowDuration);

        _slowedEnemies.Remove(enemy);
        if(!_slowedEnemies.Contains(enemy))
            enemy._agent.speed = defaultWalkSpeed;
    }
    public override void ApplyDamage(Enemy enemy, string identifier, float damage = -1, DamageType damageType = DamageType.Physical)
    {
        StartCoroutine(Poison(enemy, poisonDamage, poisonDuration, poisonInterval));
        
        if(_coatActive)
            StartCoroutine(Slow(enemy, coatSlowRate, coatSlowDuration));
        if (_strongPoisonActive)
            damageType = DamageType.Poison;

        base.ApplyDamage(enemy, identifier, damage, damageType);
    }

    public override void TakeDamage(float damage, GameObject damager, DamageType damageType)
    {
        if (_evasionActive)
        {
            int roll = Random.Range(0, 100);
            if (roll < evasionDodgeRate)
                return;
        }
        
        base.TakeDamage(damage, damager, damageType);
    }
}
