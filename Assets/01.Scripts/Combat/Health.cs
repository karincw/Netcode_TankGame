using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Health : NetworkBehaviour
{
    [SerializeField] private ParticleSystem _explosionParticle;
    public NetworkVariable<int> currentHealth;
    public int maxHealth;

    public event Action OnDieEvent;
    public event Action OnHealthChangedEvent;

    private bool _isDead;

    private void Awake()
    {
        currentHealth = new NetworkVariable<int>();
    }

    public override void OnNetworkSpawn()
    {
        //���ǻ���2 . NetworkVariable�� ������ �ǵ帱 �� �ִ�. Ŭ��� ���� ������ �ޱ⸸
        if (IsClient)
        {
            currentHealth.OnValueChanged += HandleHealthValueChanged;
        }

        if (IsServer == false) return;
        currentHealth.Value = maxHealth; //ó�� ���۽� �ִ�ü������ �־��ش�.
    }

    public override void OnNetworkDespawn()
    {

        for(int i = 0; i < 4; i++)
        {
            Vector2 pos = (Vector2)transform.position 
                                    + Random.insideUnitCircle;
            Instantiate(_explosionParticle, pos, Quaternion.identity);
        }

        if (IsClient)
        {
            currentHealth.OnValueChanged -= HandleHealthValueChanged;
        }

    }

    public float GetNormalizedHealth()
    {
        return (float)currentHealth.Value / maxHealth;
    }

    private void HandleHealthValueChanged(int previousValue, int newValue)
    {
        OnHealthChangedEvent?.Invoke();

        int delta = newValue - previousValue;
        int value = Mathf.Abs(delta);

        if (value == maxHealth) return; //ó�� ����� �� ü�� ä������ 

        Color textColor = delta < 0 ? Color.red : Color.green;
        TextManager.Instance.PopUpText(
            value.ToString(), transform.position, textColor);
    }

    //�̳༮�� ������ �����ϴ� �ż����
    private void ModifyHealth(int value)
    {
        if (_isDead) return;

        currentHealth.Value = Mathf.Clamp(currentHealth.Value + value, 0, maxHealth);

        if(currentHealth.Value == 0)
        {
            OnDieEvent?.Invoke();
            _isDead = true;
        }
    }

    [ClientRpc]
    public void ShowTextClientRpc(string text, Color color)
    {
        TextManager.Instance.PopUpText(text, transform.position, color);
    }

    public void TakeDamage(int damageValue)
    {
        if (MapManager.Instance.IsInSafetyZone(transform.position))
        {
            ShowTextClientRpc("Invincible", Color.white);
            return;
        }

        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }
}
