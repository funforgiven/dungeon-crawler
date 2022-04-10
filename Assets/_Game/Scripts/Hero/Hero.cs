using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour, IDamageable
{
    [Header("Attack")]
    [SerializeField] protected GameObject swordPrefab;
    [SerializeField] protected float swordDamage = 10f;
    [SerializeField] public float critRate = 0f;
    [SerializeField] public float critDamage = 200f;

    protected Sword sword;

    [Header("Movement")]
    [SerializeField] protected float defaultWalkSpeed = 4f;
    [SerializeField] protected GameObject sprint;
    protected float walkSpeed;
    protected float _inputHorizontal;
    protected float _inputVertical;
    protected Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    protected Animator _animator;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float healthRegen = 1f;
    [SerializeField] private float healthRegenCooldown = 3f;
    private float _healthRegenCurrentCooldown = 0f;
    protected float _health;
    private Slider healthBar;
    private bool isDead = false;

    [Header("Dash")]
    [SerializeField] private float dashCooldown = 5f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashSpeed = 15f;

    protected DashState _dashState;
    private float _dashCurrentTime;
    private float _dashCurrentCooldown;

    protected Vector2 _savedVelocity;
    protected StabState _stabState;

    [Header("UI")]
    [SerializeField] private GameObject userInterface;
      private GameObject ThisUI;

    protected virtual void Start()
    {
    	ThisUI = Instantiate(userInterface);
		healthBar = GameObject.FindWithTag("HPBar").GetComponent<Slider>();

        _health = maxHealth;
		walkSpeed = defaultWalkSpeed;

        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        swordPrefab = Instantiate(swordPrefab, transform.position, Quaternion.identity);
        swordPrefab.transform.SetParent(transform);

        sword = swordPrefab.GetComponent<Sword>();
        sword.owner = this;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        healthBar.value = _health / maxHealth;
        _inputHorizontal = Input.GetAxisRaw("Horizontal");
        _inputVertical = Input.GetAxisRaw("Vertical");

        _healthRegenCurrentCooldown += Time.deltaTime;

        if (_healthRegenCurrentCooldown > healthRegenCooldown)
        {
            _health += Time.deltaTime * healthRegen;
        }

        switch (_dashState)
        {
            case DashState.Ready:
                if (Input.GetKeyDown(KeyCode.Space) && _stabState != StabState.Stabbing)
                {
                    _savedVelocity = new Vector2(_inputHorizontal, _inputVertical).normalized;

                    if (_savedVelocity.magnitude > 0)
                    {
                        _dashState = DashState.Dashing;
                    }
                }
                break;
            case DashState.Dashing:
                _dashCurrentTime += Time.deltaTime;
                if(_dashCurrentTime >= dashTime)
                {
                    sword.Disable();
                    _dashCurrentTime = 0f;
                    _dashState = DashState.Cooldown;
                }
                break;
            case DashState.Cooldown:
                _dashCurrentCooldown += Time.deltaTime;
                if(_dashCurrentCooldown >= dashCooldown)
                {
                    _dashCurrentCooldown = 0f;
                    _dashState = DashState.Ready;
                }
                break;
        }
    }

    protected virtual void FixedUpdate()
    {
        var velocity = new Vector2(_inputHorizontal, _inputVertical);
        _rigidbody.velocity = velocity.normalized * walkSpeed;

        _animator.SetBool("Move", velocity.magnitude > 0);

        if (velocity.x < 0)
        {
            _spriteRenderer.flipX = true;
            sprint.GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (velocity.x > 0)
        {
            _spriteRenderer.flipX = false;
            sprint.GetComponent<SpriteRenderer>().flipX = false;
        }

        if (_dashState == DashState.Dashing)
            _rigidbody.velocity = _savedVelocity.normalized * dashSpeed;
    }

    public virtual void TakeDamage(float damage, GameObject damager, DamageType damageType)
    {
        _health -= damage;
        _healthRegenCurrentCooldown = 0f;

        if (!isDead && _health <= 0)
        {
            isDead = true;
            OnDeath(damager);
        }
    }

    public virtual void ApplyDamage(Enemy enemy, string identifier, float damage = -1)
    {
        if (damage == -1)
            damage = swordDamage;

        int roll = Random.Range(0, 100);
        if (roll < critRate)
        {
            enemy.TakeDamage(damage * (critDamage/100), gameObject, DamageType.Physical);
        }
        else
            enemy.TakeDamage(damage, gameObject, DamageType.Physical);
    }

    public void OnDeath(GameObject killer)
    {
      Destroy(gameObject);
      Destroy(ThisUI);  
        GameManager.Instance.StartGame(killer.GetComponent<Enemy>().hero);

    }
}
