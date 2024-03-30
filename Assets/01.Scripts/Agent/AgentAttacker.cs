using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class AgentAttacker : MonoBehaviour
{
    [HideInInspector] public Agent _agent;

    [Header("Refs")]
    [SerializeField] private Transform _fireTrm;
    [SerializeField] private GameObject _clientProjectilePrefab;
    [SerializeField] private GameObject _serverProjectilePrefab;
    [SerializeField] private Collider2D _playerCollider;

    [Header("Settings")]
    [SerializeField] private float _bulletSpeed;
    public float AttackDelayTime = 3;

    public event Action OnFireEvent;

    public void Fire()
    {

        FireServerRpc(_fireTrm);
        SpawnDummyProjectile(_fireTrm);

    }

    private void SpawnDummyProjectile(Transform fireTrm)
    {
        var projectile = Instantiate(_clientProjectilePrefab, fireTrm.position, fireTrm.rotation);

        Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(_playerCollider, projectileCollider);

        projectile.GetComponent<Rigidbody2D>().velocity = projectile.transform.right * _bulletSpeed;

        OnFireEvent?.Invoke();
    }

    [ServerRpc]
    private void FireServerRpc(Transform fireTrm)
    {
        var projectile = Instantiate(_serverProjectilePrefab, fireTrm.position, fireTrm.rotation);

        Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(_playerCollider, projectileCollider);

        projectile.GetComponent<Rigidbody2D>().velocity = projectile.transform.right * _bulletSpeed;

        SpawnDummyProjectileClientRpc(fireTrm);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Transform fireTrm)
    {
        if (_agent.IsOwner) return;

        SpawnDummyProjectile(fireTrm);
    }
}
