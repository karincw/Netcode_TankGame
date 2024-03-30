using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AgentHealth : NetworkBehaviour
{
    NetworkVariable<float> currentHealth;
    [SerializeField] private float _maxHealth;
    [SerializeField] private bool _isDead;

    public event Action OnDieEvent;
    public event Action OnHealthChanged;

    private void Start()
    {
        currentHealth = new NetworkVariable<float>();
    }

    public override void OnNetworkSpawn()
    {

    }
    public override void OnNetworkDespawn()
    {

    }

    //이 녀석은 서버만 실행하는 매서드야
    private void ModifyHealth(int value)
    {
        if (_isDead) return;
        currentHealth.Value = Mathf.Clamp(currentHealth.Value + value, 0, _maxHealth);

        if (currentHealth.Value == 0)
        {
            OnDieEvent?.Invoke();
            _isDead = true;
        }
    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }
}
