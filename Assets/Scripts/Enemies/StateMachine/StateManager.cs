using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> _states;
    protected BaseState<EState> _currentState;
    
    private void Start()
    {
        _currentState?.EnterState();
    }

    private void Update()
    {
        if (_currentState == null) return;
        
        _currentState.UpdateState();

        EState nextStateKey = _currentState.GetNextState();
        if (!nextStateKey.Equals(_currentState.stateKey))
        {
            TransitionToState(nextStateKey);
        }
    }

    private void TransitionToState(EState nextStateKey)
    {
        _currentState?.ExitState();
        _currentState = _states[nextStateKey];
        _currentState?.EnterState();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _currentState?.OnTriggerEnter2D(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        _currentState?.OnTriggerStay2D(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _currentState?.OnTriggerExit2D(other);
    }
}
