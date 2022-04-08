using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour, IDamageable
{
    [Header("Attack")]
    [SerializeField] protected GameObject currentWeapon;
    [SerializeField] public float critRate = 0f;
    [SerializeField] public float critDamage = 200f;

    [Header("Movement")]
    [SerializeField] protected float defaultWalkSpeed = 4f;
    [SerializeField] protected GameObject sprint;
    protected float walkSpeed;
    protected float _inputHorizontal;
    protected float _inputVertical;
    protected Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float healthRegen = 1f;
    [SerializeField] private float healthRegenCooldown = 3f;
    private float _healthRegenCurrentCooldown = 0f;
    protected float _health;
    private Slider healthBar;

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

        currentWeapon = Instantiate(currentWeapon, transform.position, Quaternion.identity);
        currentWeapon.transform.SetParent(transform);
        currentWeapon.GetComponent<Weapon>().owner = this;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        healthBar.value = _health / maxHealth;
        _inputHorizontal = Input.GetAxisRaw("Horizontal");
        _inputVertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentWeapon.GetComponent<Weapon>().Attack();
        }
        
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
            sprint.GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (velocity.x > 0)
        {
            _spriteRenderer.flipX = false;
            sprint.GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    public virtual void TakeDamage(float damage, GameObject damager)
    {
        _health -= damage;
        _healthRegenCurrentCooldown = 0f;
        
        if (_health <= 0) OnDeath(damager);
    }

    public void OnDeath(GameObject killer)
    {
        GameManager.Instance.StartGame(killer.GetComponent<Enemy>().hero);
        Destroy(gameObject);
    }
}
