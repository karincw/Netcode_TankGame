using System;
using UnityEngine;

public class PlayerIdleState : CanShootingState
{
    public PlayerIdleState(Agent agent) : base(agent)
    {

    }


    public override void Enter()
    {
        base.Enter();
        if (_agent.IsOwner)
        {
            _agent.Input.OnMovementEvent += HandleMovementEvent;
        }
        _agent.Movement.SetMovement(Vector2.zero);
    }

    public override void Exit()
    {
        if (_agent.IsOwner)
        {
            _agent.Input.OnMovementEvent -= HandleMovementEvent;
        }
        base.Exit();
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    private void HandleMovementEvent(Vector2 movement)
    {
        if (movement.sqrMagnitude >= 0.05f)
        {
            _agent.ChangeState(PlayerFSMState.Move);
        }
    }

}
