using UnityEngine;

public class PlayerMoveState : CanShootingState
{
    public PlayerMoveState(Agent agent) : base(agent)
    {
    }

    Vector2 direction;

    public override void Enter()
    {
        base.Enter();
        if (_agent.IsOwner)
        {
            _agent.Input.OnMovementEvent += HandleMovementEvent;
        }
    }

    public override void Exit()
    {
        if (_agent.IsOwner)
        {
            _agent.Input.OnMovementEvent -= HandleMovementEvent;
        }
        direction = Vector2.zero;
        base.Exit();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        _agent.Movement.SetMovement(direction);
    }

    private void HandleMovementEvent(Vector2 movement)
    {
        //Debug.Log("Handle");
        if (movement.sqrMagnitude <= 0.05f)
        {
            _agent.ChangeState(PlayerFSMState.Idle);
        }
        else
        {
            direction = movement;
        }
    }
}
