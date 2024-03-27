using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanShootingState : PlayerState
{
    public CanShootingState(Agent agent) : base(agent)
    {
    }

    bool _shootingState = false;
    float _lastShootingTime = 0;

    public override void Enter()
    {
        base.Enter();
        
        if (_agent.IsOwner)
        {
            _agent.Input.OnFireEvent += HandleFireEvent;
        }
    }

    public override void Exit()
    {
        if (_agent.IsOwner)
        {
            _agent.Input.OnFireEvent -= HandleFireEvent;
        }

        base.Exit();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        Rotation();

        Shooting();
    }

    private void HandleFireEvent(bool state)
    {
        _shootingState = state;
    }

    private void Shooting()
    {
        //���¹ٲ𶧸��� ���ε��µ� �̰ŵ� ����� ���� ����� �ɵ�
        if (_shootingState == true && (Time.time - _lastShootingTime) >= _agent.Attacker.AttackDelayTime)
        {
            _lastShootingTime = Time.time;

            _agent.Attacker.Fire();

        }
    }

    private void Rotation()
    {
        Vector2 mousePos;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        _agent.Movement.CannonLookPosition(mousePos - (Vector2)_agent.transform.position);

    }
}
