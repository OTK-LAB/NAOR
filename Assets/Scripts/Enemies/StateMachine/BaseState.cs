using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState<EState> where EState : Enum
{
    public EState stateKey { get; private set; }

    public BaseState(EState key)
    {
        stateKey = key;
    }
    
    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract EState GetNextState();
    public abstract void OnTriggerEnter2D(Collider2D other);
    public abstract void OnTriggerStay2D(Collider2D other);
    public abstract void OnTriggerExit2D(Collider2D other);
}
