using UnityEngine;

public class PlayerMoveState : PlayerState
{

    public PlayerMoveState(Agent agent) : base(agent)
    {
    }

    Vector2 direction;

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
        _agent.Movement.SetMove(direction);
    }

    private void HandleMovementEvent(Vector2 movement)
    {
        if (movement.sqrMagnitude <= 0.05f)
        {
            _agent.ChangeState(PlayerFSMState.Idle);
        }
        else
        {
            direction = movement.normalized;
        }
    }
}
