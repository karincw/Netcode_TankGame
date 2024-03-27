using Unity.Netcode;
using UnityEngine;

public class AgentMovement : NetworkBehaviour
{
    [HideInInspector] public Agent _agent;

    [Header("Refs")]
    [SerializeField] private Transform _bodyTrm;
    [SerializeField] private Transform _cannonTrm;
    private Rigidbody2D _rigidbody;

    [Header("Settings")]
    [SerializeField] private float _movementSpeed = 4f; //이동속도
    [SerializeField] private float _turningSpeed = 30f; //회전속도

    private Vector2 _movementInput;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetMovement(Vector2 movement)
    {
        _movementInput = movement;
    }

    public void CannonLookPosition(Vector2 direction)
    {
        float dir;
        dir = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _cannonTrm.rotation = Quaternion.Euler(0, 0, dir);
    }

    private void Update()
    {
        float zRotation = _movementInput.x * -_turningSpeed * Time.deltaTime;
        transform.Rotate(0, 0, zRotation);
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = _bodyTrm.up * (_movementInput.y * _movementSpeed);
    }
}
