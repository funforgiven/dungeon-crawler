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

    [Header("UI")] 
    [SerializeField] private GameObject userInterface;

    protected virtual void Start()
    {
      	Instantiate(userInterface);
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
    }
    
    protected virtual void FixedUpdate()
    {
        var velocity = new Vector2(_inputHorizontal, _inputVertical);
        _rigidbody.velocity = velocity.normalized * walkSpeed;

        _animator.SetBool("Move", velocity.magnitude > 0);

        if (velocity.x < 0)
        {
            _spriteRenderer.flipX = true;
            //sprint.GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (velocity.x > 0)
        {
            _spriteRenderer.flipX = false;
            //sprint.GetComponent<SpriteRenderer>().flipX = false;
        }
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

    public virtual void ApplyDamage(Enemy enemy, string identifier)
    {
        
        int roll = Random.Range(0, 100);
        if (roll < critRate)
        {
            enemy.TakeDamage(swordDamage * (critDamage/100), gameObject, DamageType.Physical);
        }
        else
            enemy.TakeDamage(swordDamage, gameObject, DamageType.Physical);
    }
    
    public void OnDeath(GameObject killer)
    {
        GameManager.Instance.StartGame(killer.GetComponent<Enemy>().hero);
        Destroy(gameObject);
    }
}
