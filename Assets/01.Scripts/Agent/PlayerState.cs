using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStateEnum : int
{
    Idle,
    Move,
    Shoot,
    Dash
}


public class PlayerState : MonoBehaviour
{
    protected Agent _agent;

    public virtual void Enter()
    {

    }
    public virtual void Exit()
    {

    }
    public virtual void UpdateState()
    {

    }

    public PlayerState(Agent agent)
    {
        this._agent = agent;
    }
}
