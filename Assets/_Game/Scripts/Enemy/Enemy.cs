using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Agent")]
    protected NavMeshAgent _agent;
    protected bool _isShooting = false;
    protected float _health;
    protected Transform _target;
    
    private Transform _transform;
    private SpriteRenderer _spriteRenderer;
    
    [SerializeField] protected float chaseRange = 5f;
    [SerializeField] protected float shootRange = 7f;

    [Header("Health")]
    [SerializeField] protected float maxHealth = 100;
    
    void Start()
    {
        _target = GameObject.FindWithTag("Player").transform;

        _transform = transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _health = maxHealth;
    }
    
    protected virtual void Update()
    {
        _spriteRenderer.flipX = _transform.position.x < _target.position.x;
        
        var distanceToPlayer = Vector2.Distance(_target.position, transform.position);
        if (!_isShooting)
        {
            if (distanceToPlayer <= chaseRange)
            {
                _agent.isStopped = true;
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

    public void TakeDamage(float damage)
    {
        _health -= damage;
        
        if(_health <= 0)
            Destroy(gameObject);
    }
}
