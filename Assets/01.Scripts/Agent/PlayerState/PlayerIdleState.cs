using System;
using UnityEngine;

public class PlayerIdleState : PlayerState
{

    public PlayerIdleState(Agent agent) : base(agent)
    {

    }

    public override void Enter()
    {
        base.Enter();
        _agent.Input.OnMovementEvent += HandleMovementEvent;
    }

    public override void Exit()
    {
        _agent.Input.OnMovementEvent -= HandleMovementEvent;
        base.Exit();
    }

    public override void UpdateState()
    {

    }

    private void HandleMovementEvent(Vector2 movement)
    {
        if(movement.sqrMagnitude >= 0.05f)
        {
            _agent.ChangeState(PlayerFSMState.Move);
        }
    }

}
