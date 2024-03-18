using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_Hit : ArcherState
{
    private Archer.State nextState = Archer.State.STATE_HIT;
    private const float knockbackDistance = -0.5f;
    private float hitTimer = 0.0f;
    private Animator archerAnimator;

    public Archer_Hit(GameObject _archerGameObject, GameObject _playerGameObject):
        base(Archer.State.STATE_HIT, _archerGameObject, _playerGameObject)
    {
        archerAnimator = _archerGameObject.GetComponent<Animator>();
    }

    public override void EnterState()
    {
        Vector2 knockbackVector = moveRight ? Vector2.right : Vector2.left;
        archerRigidBody.MovePosition(archerRigidBody.position + knockbackVector * knockbackDistance);
        
        archerComponent.ChangeAnimationState("hit");
        hitTimer = 0.0f;
        nextState = Archer.State.STATE_HIT;
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        hitTimer += Time.deltaTime;
        if (hitTimer >= archerAnimator.GetCurrentAnimatorStateInfo(0).length)
        {
            nextState = Archer.State.STATE_COOLDOWN;
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
