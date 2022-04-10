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

    [SerializeField] protected float chaseRange = 5f;
    [SerializeField] protected float shootRange = 7f;

    [Header("Health")]
    [SerializeField] protected float maxHealth = 100;
    [SerializeField] private HPBar Healthbar;
    [SerializeField]public Slider Bar;

    [Header("Hero")]
    [SerializeField] public GameObject hero;

    [Header("Event")]
    public Event _event = null;

    void Start()
    {
        _target = GameObject.FindWithTag("Player").transform;

        _transform = transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _health = maxHealth;


    }

    protected virtual void Update()
    {
      Bar.value= _health / maxHealth;
      Bar.transform.SetParent(transform);

        _spriteRenderer.flipX = _transform.position.x < _target.position.x;
        _animator.SetBool("Move", _agent.velocity.magnitude > 0);
        Healthbar.slider.value = _health / maxHealth;
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
