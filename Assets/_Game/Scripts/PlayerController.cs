using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 4f;

    private float _inputHorizontal;
    private float _inputVertical;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _inputHorizontal = Input.GetAxisRaw("Horizontal");
        _inputVertical = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        var velocity = new Vector2(_inputHorizontal, _inputVertical);
        _rb.velocity = velocity.normalized * walkSpeed;
    }
}
