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
    [SerializeField] private float sprintMaxDuration = 8f;
    private float _sprintPercentage;
    private Slider _pSprint;
    private float _sprintCurrentDuration;

    [Header("Burn")]
    [SerializeField] private float burnDamage = 3f;
    [SerializeField] private int burnDuration = 4;
    [SerializeField] private float burnInterval = 2f;
    private List<Enemy> _burningEnemies = new List<Enemy>();

    [Header("Fire Sword")]
    [SerializeField] private Sprite fireSwordSprite;
    [SerializeField] private float fireSwordSprintDuration = 1.5f;
    [SerializeField] private float fireSwordCooldown = 10f;
    private bool _fireSwordOnCooldown = false;
    private float _fireSwordCurrentCooldown = 0f;

    [Header("Fireball")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballCooldown = 10f;
    [SerializeField] private float fireballSpeed = 3f;
    [SerializeField] private float fireballDamage= 3f;
    [SerializeField] private float fireballSprintDuration = 4f;
    private GameObject _fireball;
    private bool _fireballOnCooldown = false;
    private float _fireballCurrentCooldown = 0f;

    [Header("Flame Barrier")]
    [SerializeField] private float flameBarrierCooldown;
    [SerializeField] private int flameBarrierCharge = 3;
    [SerializeField] private float flameBarrierRegeneration = 2f;
    [SerializeField] private float flameBarrierDamage = 25f;
    [SerializeField] private float flameBarrierDamageRadius = 6f;
    [SerializeField] private float flameBarrierSprintDuration = 4f;
    [SerializeField] private LayerMask flameBarrierDamageLayer;
    private TMP_Text _barrierCounter;
    private int _flameBarrierCurrentCharge;
    private bool _flameBarrierActive;
    private bool _flameBarrierOnCooldown = false;
    private float _flameBarrierCurrentCooldown = 0f;

    [Header("Big Fireball")]
    [SerializeField] private GameObject bigFireballPrefab;
    [SerializeField] private float bigFireballCooldown = 5f;
    [SerializeField] private float bigFireballDamage = 25f;
    [SerializeField] private float bigFireballDamageRadius = 3f;
    [SerializeField] private float bigFireballSprintDuration = 4f;
    [SerializeField] private LayerMask bigFireballDamageLayer;
    private bool _bigFireballOnCooldown = false;
    private float _bigFireballCurrentCooldown = 0f;

    protected override void Start()
    {
        base.Start();
        _barrierCounter = GameObject.FindWithTag("Counter").GetComponent<TMP_Text>();
        _pSprint = GameObject.FindWithTag("SprintBar").GetComponent<Slider>();
        _sprintCurrentDuration = sprintMaxDuration;
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            sword.Attack();
        }

        _sprintPercentage = (sprintMaxDuration - _sprintCurrentDuration) / sprintMaxDuration;
        _pSprint.value = _sprintPercentage;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (!_fireSwordOnCooldown)
            {
                _fireSwordOnCooldown = true;
                _fireSwordCurrentCooldown = 0;

                sword.Attack("Burn", fireSwordSprite);

                Sprint(fireSwordSprintDuration);
            }
        }

        if (_fireSwordOnCooldown)
        {
            _fireSwordCurrentCooldown += Time.deltaTime;
            rcCooldown.fillAmount = (fireSwordCooldown - _fireSwordCurrentCooldown) / fireSwordCooldown;

            if (_fireSwordCurrentCooldown > fireSwordCooldown)
            {
                _fireSwordCurrentCooldown = 0;
                _fireSwordOnCooldown = false;
            }
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

                Sprint(fireballSprintDuration);
            }
        }

        if (_fireballOnCooldown)
        {
            _fireballCurrentCooldown += Time.deltaTime;
            qCooldown.fillAmount = (fireballCooldown - _fireballCurrentCooldown) / fireballCooldown;

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
                _barrierCounter.text = _flameBarrierCurrentCharge.ToString();
                _animator.SetBool("Flame Barrier", true);

                Sprint(flameBarrierSprintDuration);
            }
        }

        if (_flameBarrierOnCooldown)
        {
            _flameBarrierCurrentCooldown += Time.deltaTime;
            eCooldown.fillAmount = (flameBarrierCooldown - _flameBarrierCurrentCooldown) / flameBarrierCooldown;

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
            if (!_bigFireballOnCooldown)
            {
                _bigFireballOnCooldown = true;

                var location = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Instantiate(bigFireballPrefab, location, Quaternion.identity);
                var overlap = Physics2D.OverlapCircleAll(location, bigFireballDamageRadius, bigFireballDamageLayer);

                foreach (var col in overlap)
                {
                    var enemy = col.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(bigFireballDamage, gameObject, DamageType.Magical);
                        StartCoroutine(Burn(enemy, burnDamage, burnDuration, burnInterval));
                    }
                }

                Sprint(bigFireballSprintDuration);
            }
        }

        if (_bigFireballOnCooldown)
        {
            _bigFireballCurrentCooldown += Time.deltaTime;
rCooldown.fillAmount = (bigFireballCooldown - _bigFireballCurrentCooldown) / bigFireballCooldown;

            if (_bigFireballCurrentCooldown > bigFireballCooldown)
            {
                _bigFireballCurrentCooldown = 0;
                _bigFireballOnCooldown = false;
            }
        }

        if (_sprintCurrentDuration < sprintMaxDuration)
        {
            sprint.GetComponent<SpriteRenderer>().enabled = true;
            _sprintCurrentDuration += Time.deltaTime;
            walkSpeed = defaultWalkSpeed * sprintSpeed;
        }
        else
        {
            sprint.GetComponent<SpriteRenderer>().enabled = false;
            walkSpeed = defaultWalkSpeed;
        }
    }

    private void Sprint(float duration)
    {
        _sprintCurrentDuration = Mathf.Clamp(_sprintCurrentDuration - duration, 0, sprintMaxDuration);
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
            enemy.TakeDamage(damage * (_sprintPercentage + 1), gameObject, DamageType.Fire);
            yield return new WaitForSeconds(cooldown);
        }

        _burningEnemies.Remove(enemy);
    }

    public override void ApplyDamage(Enemy enemy, string identifier = null, float damage = -1, DamageType damageType = DamageType.Physical)
    {
        if (identifier == "Burn")
        {
            StartCoroutine(Burn(enemy, burnDamage, burnDuration, burnInterval));
        }

        base.ApplyDamage(enemy, identifier, damage, damageType);
    }

    public override void TakeDamage(float damage, GameObject damager, DamageType damageType)
    {
        if (_flameBarrierActive)
        {
            _flameBarrierCurrentCharge -= 1;
              _barrierCounter.text = _flameBarrierCurrentCharge.ToString();
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
