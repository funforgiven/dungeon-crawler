using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private GameObject currentWeapon;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4f;
    private float _inputHorizontal;
    private float _inputVertical;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    
    [Header("Health")]
    [SerializeField] private float maxHealth = 100;
    private float _health;
        
    private void Start()
    {
        _health = maxHealth;
        
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        currentWeapon = Instantiate(currentWeapon, transform.position, Quaternion.identity);
        currentWeapon.transform.SetParent(transform);
    }

    // Update is called once per frame
    void Update()
    {
        _inputHorizontal = Input.GetAxisRaw("Horizontal");
        _inputVertical = Input.GetAxisRaw("Vertical");
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentWeapon.GetComponent<Weapon>().Attack(this.gameObject);
        }
    }
    
    private void FixedUpdate()
    {
        var velocity = new Vector2(_inputHorizontal, _inputVertical);
        _rigidbody.velocity = velocity.normalized * walkSpeed;

        if (velocity.x < 0)
            _spriteRenderer.flipX = true;
        else if (velocity.x > 0)
            _spriteRenderer.flipX = false;
    }
    
    public void TakeDamage(float damage)
    {
        _health -= damage;
        
        if(_health <= 0)
            Destroy(gameObject);
    }
}
