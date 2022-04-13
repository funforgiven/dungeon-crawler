using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Agent")]
    [HideInInspector] public NavMeshAgent _agent;
    internal bool _isAttacking = false;
    internal bool _inCC = false;
    protected float _health;
    internal Transform _target;

    private Transform _transform;
    private SpriteRenderer _spriteRenderer;
    internal Animator _animator;

    [Header("Movement")]
    [SerializeField] protected float chaseRange = 5f;
    [SerializeField] protected float attackRange = 7f;
    [SerializeField] protected float detectionRange = 10f;
    [SerializeField] protected float defaultWalkSpeed = 3f;

    [Header("Health")]
    [SerializeField] protected float maxHealth = 100;
    [SerializeField] private Canvas healthCanvas;
    [SerializeField] private float healthBarOffset = 20f;
    private Slider _healthBar;

    [Header("Attack")] 
    [SerializeField] protected float attackRate = 20f;
    [SerializeField] protected GameObject swordPrefab;
    [SerializeField] public float swordDamage;
    protected EnemySword sword;
    protected float _attackTimeElapsed = 0f;
    
    [Header("Hero")]
    [SerializeField] public GameObject hero;

    [Header("Event")]
    public Event _event = null;

    protected virtual void Start()
    {
        healthCanvas = Instantiate(healthCanvas, transform.position, Quaternion.identity);
        healthCanvas.transform.SetParent(transform);
        _healthBar = healthCanvas.GetComponentInChildren<Slider>();
        _healthBar.transform.localPosition = new Vector3(0, healthBarOffset, 0);
        _health = maxHealth;
        
        _target = GameObject.FindWithTag("Player").transform;

        _transform = transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.speed = defaultWalkSpeed;
        _agent.enabled = false;
    }

    protected virtual void Update()
    {
        _healthBar.value = _health / maxHealth;

        _spriteRenderer.flipX = _transform.position.x < _target.position.x;
        _animator.SetBool("Move", _agent.velocity.magnitude > 0);
        
        var distanceToPlayer = Vector2.Distance(_target.position, transform.position);

        if (!_agent.enabled && distanceToPlayer < detectionRange)
        {
            _agent.enabled = true;
            FirstEncounter();
        }
        else if(!_agent.enabled)
            return;
        
        if (_inCC)
        {
            _isAttacking = false;
        }
        else if (!_isAttacking)
        {
            if (distanceToPlayer <= chaseRange)
            {
                _agent.ResetPath();
                _isAttacking = true;
            }
            else
            {
                _agent.SetDestination(_target.position);
            }
        }
        else
        {
            if (distanceToPlayer > attackRange)
            {
                _isAttacking = false;
            }
        }
    }

    public virtual void TakeDamage(float damage, GameObject damager, DamageType damageType)
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

    protected virtual IEnumerator FirstEncounter()
    {
        yield return null;
    }

}
