using System;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    [HideInInspector] public Agent Agent;

    private Rigidbody2D _rigidbody;

    #region

    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float maxMoveSpeed = 60f;

    #endregion

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = Vector2.ClampMagnitude(_rigidbody.velocity, maxMoveSpeed);
        _rigidbody.velocity = Vector2.Lerp(_rigidbody.velocity, Vector2.zero, rotationSpeed * Time.fixedDeltaTime);
        //작동안하는 **련

    }

    public void SetMove(Vector2 dir)
    {
        //_rigidbody.AddForce(transform.right * moveSpeed, ForceMode2D.Force);

        if(dir.x > 0) // -로
        {
            transform.rotation =
                Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, transform.rotation.z - 360), rotationSpeed);
        }
        else if (dir.x < 0)
        {
            transform.rotation =
                Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, transform.rotation.z + 360), rotationSpeed);
        }
        //작동안하는 **련들

        if (dir.y < 0)
        {
            _rigidbody.AddForce(transform.right * -moveSpeed, ForceMode2D.Force);
        }
        else if(dir.y > 0)
        {
            _rigidbody.AddForce(transform.right * moveSpeed, ForceMode2D.Force);
        }
    }

    public void StopImmediately()
    {
        _rigidbody.velocity = Vector2.zero;
    }
}
