using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerFSMState
{
    Idle = 1,
    Move,
    Dash,
    Shoot,
}

public class Agent : MonoBehaviour
{

    public InputSO Input;
    [HideInInspector] public AgentMovement Movement;
    [HideInInspector] public AgentAnimator Animator;

    PlayerFSMState myState = PlayerFSMState.Idle;
    private Dictionary<PlayerFSMState, PlayerState> StateMachine = new();
    private PlayerState currentState;

    private void Awake()
    {
        var visualTrm = transform.Find("Visual");
        Movement = GetComponent<AgentMovement>();
        Animator = visualTrm.GetComponent<AgentAnimator>();

        foreach (PlayerFSMState stateEnum in Enum.GetValues(typeof(PlayerFSMState)))
        {
            string typeName = stateEnum.ToString();

            try
            {
                Type t = Type.GetType($"Player{typeName}State");
                PlayerState playerState = Activator.CreateInstance(t, this as object) as PlayerState;
                StateMachine.Add(stateEnum, playerState);
            }
            catch (Exception e)
            {
                Debug.LogError($"{typeName} is Errored");
                Debug.LogError(e.Message);
            }
        }
    }
    private void Start()
    {
        ChangeState(PlayerFSMState.Idle);
    }

    private void Update()
    {
        currentState.UpdateState();
    }


    public void ChangeState(PlayerFSMState State)
    {
        currentState?.Exit();
        myState = State;
        currentState = StateMachine[State];
        currentState?.Enter();
    }

}
