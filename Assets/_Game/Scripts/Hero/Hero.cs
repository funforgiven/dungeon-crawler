using UnityEngine;

public class Hero : MonoBehaviour, IDamageable
{
    [Header("Attack")]
    [SerializeField] private GameObject currentWeapon;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4f;
    private float _inputHorizontal;
    private float _inputVertical;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    
    [Header("Health")]
    [SerializeField] private float maxHealth = 100;
    private float _health;
        
    private void Start()
    {
        _health = maxHealth;
        
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        
        currentWeapon = Instantiate(currentWeapon, transform.position, Quaternion.identity);
        currentWeapon.transform.SetParent(transform);
        currentWeapon.GetComponent<Weapon>().owner = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        _inputHorizontal = Input.GetAxisRaw("Horizontal");
        _inputVertical = Input.GetAxisRaw("Vertical");
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentWeapon.GetComponent<Weapon>().Attack();
        }
    }
    
    private void FixedUpdate()
    {
        var velocity = new Vector2(_inputHorizontal, _inputVertical);
        _rigidbody.velocity = velocity.normalized * walkSpeed;

        _animator.SetBool("Move", Mathf.Abs(velocity.magnitude) > 0);

        if (velocity.x < 0)
            _spriteRenderer.flipX = true;
        else if (velocity.x > 0)
            _spriteRenderer.flipX = false;
    }

    public void TakeDamage(float damage, GameObject damager)
    {
        _health -= damage;

        if (_health > 0) return;

        GameManager.Instance.StartGame(damager.GetComponent<Enemy>().hero);
        Destroy(gameObject);
    }
}
