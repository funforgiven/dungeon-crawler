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
    protected float walkSpeed;
    protected float _inputHorizontal;
    protected float _inputVertical;
    protected Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100;
    private float _health;
    private Slider healthBar;

    [Header("UI")] 
    [SerializeField] private GameObject userInterface;

    protected virtual void Start()
    {
        _health = maxHealth;
		walkSpeed = defaultWalkSpeed;
		
        healthBar = GameObject.FindWithTag("HPBar").GetComponent<Slider>();

        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        currentWeapon = Instantiate(currentWeapon, transform.position, Quaternion.identity);
        currentWeapon.transform.SetParent(transform);
        currentWeapon.GetComponent<Weapon>().owner = this;

        Instantiate(userInterface);
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
    }
    
    protected virtual void FixedUpdate()
    {
        var velocity = new Vector2(_inputHorizontal, _inputVertical);
        _rigidbody.velocity = velocity.normalized * walkSpeed;

        _animator.SetBool("Move", velocity.magnitude > 0);

        if (velocity.x < 0)
            _spriteRenderer.flipX = true;
        else if (velocity.x > 0)
            _spriteRenderer.flipX = false;
    }

    public virtual void TakeDamage(float damage, GameObject damager)
    {
        _health -= damage;

        if (_health <= 0) OnDeath(damager);
    }

    public void OnDeath(GameObject killer)
    {
        GameManager.Instance.StartGame(killer.GetComponent<Enemy>().hero);
        Destroy(gameObject);
    }
}
