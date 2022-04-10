using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class ImpHero : Hero
{
    [Header("Passive")]
    [SerializeField] private float fireDamageReduction = 0.5f;
    [SerializeField] private float sprintSpeed = 1.5f;
    [SerializeField] private float sprintDuration = 3f;
    private float _sprintCurrentDuration;

    [Header("Fire Sword")] 
    [SerializeField] private float fireSwordDamage = 3f;
    [SerializeField] private int fireSwordDuration = 4;
    [SerializeField] private float fireSwordCooldown = 2f;
    private List<Enemy> _burningEnemies = new List<Enemy>();

    [Header("Fireball")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballCooldown = 10f;
    [SerializeField] private float fireballSpeed = 3f;
    [SerializeField] private float fireballDamage= 3f;
    private GameObject _fireball;
    private bool _fireballOnCooldown = false;
    private float _fireballCurrentCooldown = 0f;

    [Header("Flame Barrier")]
    [SerializeField] private float flameBarrierCooldown;
    [SerializeField] private int flameBarrierCharge = 3;
    [SerializeField] private float flameBarrierRegeneration = 2f;
    [SerializeField] private float flameBarrierDamage = 25f;
    [SerializeField] private float flameBarrierDamageRadius = 6f;
    [SerializeField] private LayerMask flameBarrierDamageLayer;
    [SerializeField] private TMP_Text BarrierCounter;
    private int _flameBarrierCurrentCharge;
    private bool _flameBarrierActive;
    private bool _flameBarrierOnCooldown = false;
    private float _flameBarrierCurrentCooldown = 0f;

    [Header("Big Fireball")]
    [SerializeField] private float bigFireballCooldown = 5f;
    [SerializeField] private float bigFireballDamage = 25f;
    [SerializeField] private float bigFireballDamageRadius = 3f;
    [SerializeField] private LayerMask bigFireballDamageLayer;
    private bool _bigFireballOnCooldown = false;
    private float _bigFireballCurrentCooldown = 0f;


    protected override void Start()
    {
        base.Start();

        _sprintCurrentDuration = sprintDuration;
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
            sword.Attack("Burn");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!_fireballOnCooldown)
            {
                _fireballOnCooldown = true;

                _fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);

                var direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                _fireball.GetComponent<Rigidbody2D>().velocity = direction.normalized * fireballSpeed;
                _fireball.GetComponent<Projectile>().owner = gameObject;
                _fireball.GetComponent<Projectile>()._damage = fireballDamage;

                Sprint();
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!_flameBarrierOnCooldown)
            {
                _flameBarrierActive = true;
                _flameBarrierOnCooldown = true;
                _flameBarrierCurrentCharge = flameBarrierCharge;
                BarrierCounter.text = _flameBarrierCurrentCharge.ToString();
                _animator.SetBool("Flame Barrier", true);

                Sprint();
            }
        }

        if (_flameBarrierOnCooldown)
        {
            _flameBarrierCurrentCooldown += Time.deltaTime;

            if (_flameBarrierCurrentCooldown > flameBarrierCooldown)
            {
                _flameBarrierCurrentCooldown = 0;
                _flameBarrierOnCooldown = false;
            }
        }

        if (_flameBarrierActive)
        {
            _health += Time.deltaTime * flameBarrierRegeneration;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!_flameBarrierOnCooldown)
            {
                _flameBarrierOnCooldown = true;

                var location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var overlap = Physics2D.OverlapCircleAll(location, bigFireballDamageRadius, bigFireballDamageLayer);

                foreach (var col in overlap)
                {
                    var damageable = col.GetComponent<IDamageable>();
                    if (damageable != null) damageable.TakeDamage(bigFireballDamage, gameObject, DamageType.Magical);
                }

                Sprint();
            }
        }

        if (_bigFireballOnCooldown)
        {
            _bigFireballCurrentCooldown += Time.deltaTime;

            if (_bigFireballCurrentCooldown > bigFireballCooldown)
            {
                _bigFireballCurrentCooldown = 0;
                _bigFireballOnCooldown = false;
            }
        }

        if (_sprintCurrentDuration < sprintDuration)
        {
            _sprintCurrentDuration += Time.deltaTime;
            walkSpeed = defaultWalkSpeed * sprintSpeed;
        }
        else
        {
            walkSpeed = defaultWalkSpeed;
        }
    }

    private void Sprint()
    {
        _sprintCurrentDuration = 0f;
    }

    private IEnumerator Burn(Enemy enemy, float damage, int duration, float cooldown)
    {
        if (_burningEnemies.Contains(enemy))
        {
            _burningEnemies.Add(enemy);
            yield break;
        }
        
        _burningEnemies.Add(enemy);
        int currentDuration = duration;
        
        while(currentDuration > 0)
        {
            if (_burningEnemies.Count(e => e != null && e.Equals(enemy)) == 2)
            {
                _burningEnemies.Remove(enemy);
                currentDuration = duration;
            }

            currentDuration--;
            enemy.TakeDamage(damage, gameObject, DamageType.Fire);
            yield return new WaitForSeconds(cooldown);
        }

        _burningEnemies.Remove(enemy);
    }

    public override void ApplyDamage(Enemy enemy, string identifier)
    {
        if (identifier == "Burn")
        {
            StartCoroutine(Burn(enemy, fireSwordDamage, fireSwordDuration, fireSwordCooldown));
        }
        
        base.ApplyDamage(enemy, identifier);
    }

    public override void TakeDamage(float damage, GameObject damager, DamageType damageType)
    {
        if (_flameBarrierActive)
        {
            _flameBarrierCurrentCharge -= 1;
              BarrierCounter.text = _flameBarrierCurrentCharge.ToString();
            if (_flameBarrierCurrentCharge == 0)
            {
                _flameBarrierActive = false;
                _animator.SetBool("Flame Barrier", false);
            }

            Explode();
        }

        if(damageType == DamageType.Fire) base.TakeDamage(damage * fireDamageReduction, damager, damageType);
        else base.TakeDamage(damage, damager, damageType);
    }

    private void Explode()
    {
        var overlap = Physics2D.OverlapCircleAll(transform.position, flameBarrierDamageRadius, flameBarrierDamageLayer);

        foreach (var col in overlap)
        {
            var damageable = col.GetComponent<IDamageable>();
            if(damageable != null) damageable.TakeDamage(flameBarrierDamage, gameObject, DamageType.Magical);
        }
    }
}
