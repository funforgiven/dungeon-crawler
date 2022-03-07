using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private Transform _target;
    private NavMeshAgent _agent;
    private bool _isShooting = false;

    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float shootRange = 7f;
    
    // Start is called before the first frame update
    void Start()
    {
        _target = GameObject.FindWithTag("Player").transform;
        
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
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
                _isShooting = false;
        }
    }
}
