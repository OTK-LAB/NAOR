using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_Attack : ArcherState
{
    public Archer_Attack(GameObject _archerGameObject, GameObject _playerGameObject):
        base(Archer.State.STATE_ATTACK, _archerGameObject, _playerGameObject)
    {}

    public override void EnterState()
    {
        archerRigidBody.velocity = Vector2.zero;
        archerComponent.ChangeAnimationState("attack");
    }

    public override void ExitState()
    {}

    public override void UpdateState()
    {
    }

    public override Archer.State GetNextState()
    {
        return Archer.State.STATE_ATTACK;
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {}

    public override void OnTriggerStay2D(Collider2D other)
    {}

    public override void OnTriggerExit2D(Collider2D other)
    {}
}
