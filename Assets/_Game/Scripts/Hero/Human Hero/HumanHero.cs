using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HumanHero : Hero
{
    [Header("Mana")]
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float manaRegen = 1f;
    internal float mana;
    private Slider manaBar;

    [Header("Mana Shield")]
    [SerializeField] private GameObject manaShieldPrefab;
    [SerializeField] public float manaShieldCooldown = 10f;
    internal GameObject _manaShield;
    internal bool _manaShieldOnCooldown = false;
    internal bool _manaShieldEnabled = false;
    private float _manaShieldCurrentCooldown = 0f;

    [Header("Mana Ball")]
    [SerializeField] private GameObject manaBallPrefab;
    [SerializeField] private float manaBallCooldown = 10f;
    [SerializeField] private float manaBallManaCost = 10f;
    [SerializeField] private float manaBallSpeed = 3f;
    [SerializeField] private float manaBallDamage= 3f;
    private GameObject _manaBall;
    private bool _manaBallOnCooldown = false;
    private float _manaBallCurrentCooldown = 0f;

    [Header("Run")]
    [SerializeField] private float hasteRunSpeed = 1.5f;
    [SerializeField] private float hasteSwingSpeed = 2f;
    [SerializeField] private float hasteManaCost = 1f;
    private bool _inHaste = false;

    [Header("Stab")]
    [SerializeField] private float stabCooldown = 3f;
    [SerializeField] private float stabTime = 1f;
    [SerializeField] private float stabSpeed = 3f;
    
    private float _stabCurrentTime;
    private float _stabCurrentCooldown;

    protected override void Start()
    {
        base.Start();

        mana = maxMana;
        manaBar = GameObject.FindWithTag("MPBar").GetComponent<Slider>();
    }

    protected override void Update()
    {
        base.Update();
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            sword.Attack();
        }

        mana += Time.deltaTime * manaRegen;
        manaBar.value = mana / maxMana;

        switch (_stabState)
        {
            case StabState.Ready:
                if(Input.GetKeyDown(KeyCode.Mouse1) && _dashState != DashState.Dashing)
                {
                    _savedVelocity = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
                    
                    sword.Enable();
                    var rotZ = Mathf.Atan2(_savedVelocity.y, _savedVelocity.x) * Mathf.Rad2Deg;
                    sword.transform.rotation = Quaternion.Euler(0, 0, rotZ - 90f);

                    _stabState = StabState.Stabbing;
                }
                break;
            case StabState.Stabbing:
                _stabCurrentTime += Time.deltaTime;
                if(_stabCurrentTime >= stabTime)
                {
                    sword.Disable();
                    _stabCurrentTime = 0f;
                    _stabState = StabState.Cooldown;
                }
                break;
            case StabState.Cooldown:
                _stabCurrentCooldown += Time.deltaTime;
                if(_stabCurrentCooldown >= stabCooldown)
                {
                    _stabCurrentCooldown = 0f;
                    _stabState = StabState.Ready;
                }
                break;
        }

        if (_manaShieldOnCooldown)
        {
            _manaShieldCurrentCooldown += Time.deltaTime;

            if (_manaShieldCurrentCooldown >= manaShieldCooldown)
            {
                _manaShieldOnCooldown = false;
                _manaShieldCurrentCooldown = 0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!_manaShieldOnCooldown)
            {
                _manaShield = Instantiate(manaShieldPrefab, transform.position, Quaternion.identity);
                _manaShield.transform.SetParent(transform);
            }
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            Destroy(_manaShield);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _inHaste = true;

            walkSpeed = defaultWalkSpeed * hasteRunSpeed;
            sword._swingDuration = (1/hasteSwingSpeed) * sword.defaultSwingDuration;
            sprint.GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            _inHaste = false;
            walkSpeed = defaultWalkSpeed;
            sword._swingDuration = sword.defaultSwingDuration;
            sprint.GetComponent<SpriteRenderer>().enabled = false;
        }

        if (_inHaste)
        {
            mana -= Time.deltaTime * hasteManaCost;

            if (mana <= 0)
            {
                mana = 0;
                _inHaste = false;

                walkSpeed = defaultWalkSpeed;
                sword._swingDuration = sword.defaultSwingDuration;
                sprint.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (!_manaBallOnCooldown)
            {
                if (mana >= manaBallManaCost)
                {
                    mana -= manaBallManaCost;
                    _manaBallOnCooldown = true;

                    _manaBall = Instantiate(manaBallPrefab, transform.position, Quaternion.identity);

                    var direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                    _manaBall.GetComponent<Rigidbody2D>().velocity = direction.normalized * manaBallSpeed;
                    _manaBall.GetComponent<Projectile>().owner = gameObject;
                    _manaBall.GetComponent<Projectile>()._damage = manaBallDamage;
                }
            }
        }

        if (_manaBallOnCooldown)
        {
            _manaBallCurrentCooldown += Time.deltaTime;

            if (_manaBallCurrentCooldown > manaBallCooldown)
            {
                _manaBallCurrentCooldown = 0;
                _manaBallOnCooldown = false;
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if(_stabState == StabState.Stabbing)
            _rigidbody.velocity = _savedVelocity.normalized * stabSpeed;
    }

    public override void TakeDamage(float damage, GameObject damager, DamageType damageType)
    {
        if (_manaShieldEnabled && !_manaShield)
        {
            _manaShieldEnabled = false;
            return;
        }

        if (_manaShieldEnabled)
        {
            return;
        }

        base.TakeDamage(damage, damager, damageType);
    }
}

public enum DashState
{
    Ready,
    Dashing,
    Cooldown
}

public enum StabState
{
    Ready,
    Stabbing,
    Cooldown
}
