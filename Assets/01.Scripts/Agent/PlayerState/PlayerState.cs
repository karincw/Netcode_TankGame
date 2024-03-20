using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected Agent _agent;

    public virtual void Enter()
    {

    }
    public virtual void Exit()
    {

    }
    public virtual void UpdateState() { }


    public PlayerState(Agent agent)
    {
        _agent = agent;
    }
}
