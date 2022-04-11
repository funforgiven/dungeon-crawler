using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Agent")]
    [HideInInspector] public NavMeshAgent _agent;
    internal bool _isShooting = false;
    public bool _inCC = false;
    protected float _health;
    protected Transform _target;

    private Transform _transform;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    [Header("Movement")]
    [SerializeField] protected float chaseRange = 5f;
    [SerializeField] protected float shootRange = 7f;
    [SerializeField] protected float defaultWalkSpeed = 3f;

    [Header("Health")]
    [SerializeField] protected float maxHealth = 100;
    [SerializeField] private Canvas healthCanvas;
    [SerializeField] private float healthBarOffset = 20f;
    private Slider _healthBar;

    [Header("Hero")]
    [SerializeField] public GameObject hero;

    [Header("Event")]
    public Event _event = null;

    void Start()
    {
        healthCanvas = Instantiate(healthCanvas, transform.position, Quaternion.identity);
        healthCanvas.transform.SetParent(transform);
        _healthBar = healthCanvas.GetComponentInChildren<Slider>();
        _healthBar.transform.localPosition = new Vector3(0, healthBarOffset, 0);
        
        _target = GameObject.FindWithTag("Player").transform;

        _transform = transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.speed = defaultWalkSpeed;

        _health = maxHealth;
    }

    protected virtual void Update()
    {
        _healthBar.value = _health / maxHealth;

        _spriteRenderer.flipX = _transform.position.x < _target.position.x;
        _animator.SetBool("Move", _agent.velocity.magnitude > 0);
        var distanceToPlayer = Vector2.Distance(_target.position, transform.position);

        if (_inCC)
        {
            _isShooting = false;
        }
        else if (!_isShooting)
        {
            if (distanceToPlayer <= chaseRange)
            {
                _agent.ResetPath();
                _isShooting = true;
            }
            else
            {
                _agent.SetDestination(_target.position);
            }
        }
        else
        {
            if (distanceToPlayer > shootRange)
            {
                _isShooting = false;
            }
        }
    }

    public void TakeDamage(float damage, GameObject damager, DamageType damageType)
    {
        _health -= damage;

        if(_health <= 0) OnDeath(damager);
    }

    public void OnDeath(GameObject killer)
    {
        if (_event != null)
        {
            _event.enemies.Remove(gameObject);
            if(_event.enemies.Count == 0)
                _event.OnEventEnd();
        }

        Destroy(gameObject);
    }

}
