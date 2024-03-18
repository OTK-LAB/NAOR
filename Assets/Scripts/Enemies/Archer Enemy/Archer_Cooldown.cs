using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_Cooldown : ArcherState
{
    private Archer.State nextState = Archer.State.STATE_COOLDOWN;
    
    private float duration;
    private float timer = 0.0f;

    public Archer_Cooldown(GameObject _archerGameObject, GameObject _playerGameObject, float _duration) :
        base(Archer.State.STATE_COOLDOWN, _archerGameObject, _playerGameObject)
    {
        duration = _duration;
    }

    public override void EnterState()
    {
        archerComponent.ChangeAnimationState("cooldown");
        timer = 0.0f;
        nextState = Archer.State.STATE_COOLDOWN;
    }

    public override void ExitState()
    {}

    public override void UpdateState()
    {
        timer += Time.deltaTime;
        if (timer > duration)
        {
            nextState = Archer.State.STATE_STARTINGMOVE;
        }
    }

    public override Archer.State GetNextState()
    {
        return nextState;
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {}

    public override void OnTriggerStay2D(Collider2D other)
    {}

    public override void OnTriggerExit2D(Collider2D other)
    {}
}
