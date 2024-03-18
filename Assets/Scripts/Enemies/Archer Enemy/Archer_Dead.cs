using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_Dead : ArcherState
{
    public Archer_Dead(GameObject _archerGameObject, GameObject _playerGameObject) : base(Archer.State.STATE_DEAD, _archerGameObject, _playerGameObject)
    {}

    public override void EnterState()
    {
        archerComponent.ChangeAnimationState("death");
        
        archerGameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        archerGameObject.GetComponent<Collider2D>().enabled = false;
        archerGameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
    }

    public override void ExitState()
    {}

    public override void UpdateState()
    {}

    public override Archer.State GetNextState()
    {
        return Archer.State.STATE_DEAD;
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {}

    public override void OnTriggerStay2D(Collider2D other)
    {}

    public override void OnTriggerExit2D(Collider2D other)
    {}
}
