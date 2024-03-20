using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Vector2 _velocity;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _speedIncreaseSpeed;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = _velocity;
    }

    private void SetMovement(Vector2 movement)
    {
        Vector2 normalizedVector = movement.normalized;
        Vector2 applyVector = normalizedVector * _moveSpeed;
        _velocity = applyVector;
    }

    private void SetRotation(Vector2 direction)
    {
        transform.forward = direction;
        //안될수도 있음
    }
}
