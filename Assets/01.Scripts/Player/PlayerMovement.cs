using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Transform _bodyTrm;
    [SerializeField] private ParticleSystem _dustEffect;
    private Rigidbody2D _rigidbody;

    [Header("Setting Values")]
    [SerializeField] private float _movementSpeed = 4f; //이동속도
    [SerializeField] private float _turningSpeed = 30f; //회전속도

    private Vector2 _movementInput;

    [SerializeField] private float _dustEmissionValue = 10;
    private ParticleSystem.EmissionModule _emissionModule;
    private Vector3 _prevPos;
    private float _particleStopThreshold = 0.005f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _emissionModule = _dustEffect.emission; 
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        _playerInput.OnMovementEvent += HandleMovementEvent;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        _playerInput.OnMovementEvent -= HandleMovementEvent;
    }

    private void HandleMovementEvent(Vector2 movement)
    {
        _movementInput = movement;
    }

    //Update에서는 포탑을 회전시킬꺼고-> 니네가
    private void Update()
    {
        if (!IsOwner) return;
        float zRotation = _movementInput.x * -_turningSpeed * Time.deltaTime;
        _bodyTrm.Rotate(0, 0, zRotation);
    }
    //FixedUpdate에서는 이동을 시킬꺼야 -> 같이

    private void FixedUpdate()
    {
        float moveDistance = (transform.position - _prevPos).sqrMagnitude;
        if (moveDistance > _particleStopThreshold)
        {
            _emissionModule.rateOverTime = _dustEmissionValue;
        }
        else
        {
            _emissionModule.rateOverTime = 0;
        }

        _prevPos = transform.position;

        if (!IsOwner) return;

        _rigidbody.velocity = _bodyTrm.up * (_movementInput.y * _movementSpeed);
    }

}
