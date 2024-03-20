using System;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    [HideInInspector] public Agent Agent;

    private Rigidbody2D _rigidbody;

    #region

    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float maxMoveSpeed = 60f;

    #endregion

    private float _moveDirection = 0;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rigidbody.AddForce(transform.right * moveSpeed, ForceMode2D.Force);
        _rigidbody.velocity = Vector2.ClampMagnitude(_rigidbody.velocity, maxMoveSpeed);
        Debug.Log(_rigidbody.velocity);
    }

    public void SetMove(float dir)
    {
        _moveDirection = dir;

    }

    public void StopImmediately()
    {
        _rigidbody.velocity = Vector2.zero;
    }
}
