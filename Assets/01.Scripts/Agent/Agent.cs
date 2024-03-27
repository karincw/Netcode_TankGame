using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum PlayerFSMState
{
    Idle = 1,
    Move,
    Dash,
    Shoot,
}

public class Agent : NetworkBehaviour
{
    [Header("Refs")]
    public InputSO Input;
    [HideInInspector] public AgentMovement Movement;
    [HideInInspector] public AgentAnimator Animator;
    [HideInInspector] public AgentAttacker Attacker;

    [Header("Settings")]
    private Dictionary<PlayerFSMState, PlayerState> StateMachine = new();
    [SerializeField] PlayerFSMState myState = PlayerFSMState.Idle;
    [SerializeField] private PlayerState currentState;

    private void Awake()
    {
        var visualTrm = transform.Find("Visual");
        Movement = GetComponent<AgentMovement>();
        Movement._agent = this;
        Animator = visualTrm.GetComponent<AgentAnimator>();
        Animator._agent= this;
        Attacker = GetComponent<AgentAttacker>();
        Attacker._agent = this;

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
