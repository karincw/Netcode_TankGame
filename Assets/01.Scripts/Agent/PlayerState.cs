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
    private int _animationHash;

    public virtual void Enter()
    {
        _agent.Animator.SetAnimation(_animationHash, true);
    }
    public virtual void Exit()
    {
        _agent.Animator.SetAnimation(_animationHash, false);
    }
    public virtual void UpdateState()
    {

    }

    public PlayerState(Agent agent, string animHash)
    {
        this._agent = agent;
        _animationHash = Animator.StringToHash(animHash);
    }
}
