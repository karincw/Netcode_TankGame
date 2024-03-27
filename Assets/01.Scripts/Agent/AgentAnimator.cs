using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAnimator : MonoBehaviour
{
    [HideInInspector] public Agent _agent;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
}
