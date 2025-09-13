using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSpawnCoin : Coin
{

    public event Action<ReSpawnCoin> OnCollected;

    private Vector3 _prevPos;

    public override int Collect()
    {
        if (_alreadyCollected) return 0;

        if(!IsServer)
        {
            SetVisible(false);
            return 0;
        }

        _alreadyCollected = true;
        OnCollected?.Invoke(this);
        isActive.Value = false; //������ ���ش�.

        return _coinValue;
    }

    //������ �����ϴ� �Լ�. Ŭ��� �̰� �Ƚ�����
    [ContextMenu("ResetCoin")]
    public void ResetCoin()
    {
        _alreadyCollected = false;
        isActive.Value = true; //��Ʈ��ũ ������ true�� �������ִ°���
        SetVisible(true);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _prevPos = transform.position;

        //if(IsClient)
        //    isActive.OnValueChanged += HandleActiveValueChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        //if(IsClient)
        //    isActive.OnValueChanged -= HandleActiveValueChanged;
    }

    //private void HandleActiveValueChanged(bool previousValue, bool newValue)
    //{
    //    SetVisible(newValue);
    //}

    private void Update()
    {
        if (IsServer) return;

        if(Vector2.Distance(_prevPos, transform.position) > 0.1f)
        {
            _prevPos = transform.position;
            SetVisible(true);
        }
    }
}
