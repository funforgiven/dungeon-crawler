using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject _player;
    private Rigidbody2D _rb;
    private bool _isShooting = false;

    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float shootRange = 7f;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var distanceToPlayer = Vector2.Distance(_player.transform.position, transform.position);
        if (!_isShooting)
        {
            if (distanceToPlayer <= chaseRange)
            {
                _rb.velocity = Vector2.zero;
                _isShooting = true;
            }
            else
            {
                var direction = (_player.transform.position - transform.position).normalized;
                _rb.velocity = direction * walkSpeed;
            }
            
        }
        else
        {
            if (distanceToPlayer > shootRange)
                _isShooting = false;
        }
    }
}
