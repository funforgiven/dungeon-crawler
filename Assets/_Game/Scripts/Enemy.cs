using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject _player;
    private Rigidbody2D _rb;

    [SerializeField] private float walkSpeed = 4f;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var direction = _player.transform.position - transform.position;
        _rb.transform.Translate(direction * (walkSpeed * Time.deltaTime));
    }
}
