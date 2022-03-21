using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    protected Transform Target;
    
    private Transform _transform;
    private SpriteRenderer _spriteRenderer;
    
    protected NavMeshAgent Agent;
    protected bool IsShooting = false;
    protected float Health;
    
    [SerializeField] protected float chaseRange = 5f;
    [SerializeField] protected float shootRange = 7f;

    [SerializeField] protected float maxHealth = 100;

    // Start is called before the first frame update
    void Start()
    {
        Target = GameObject.FindWithTag("Player").transform;

        _transform = transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;

        Health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        _spriteRenderer.flipX = _transform.position.x < Target.position.x;
        
        var distanceToPlayer = Vector2.Distance(Target.position, transform.position);
        if (!IsShooting)
        {
            if (distanceToPlayer <= chaseRange)
            {
                Agent.isStopped = true;
                Agent.ResetPath();
                IsShooting = true;
            }
            else
            {
                Agent.SetDestination(Target.position);
            }
            
        }
        else
        {
            if (distanceToPlayer > shootRange)
            {
                IsShooting = false;
                Attack();
            }

        }
        
    }
    
    public void TakeDamage(float damage)
    {
        Health -= damage;
        
        if(Health <= 0)
            Destroy(gameObject);
    }

    protected abstract void Attack();
}
