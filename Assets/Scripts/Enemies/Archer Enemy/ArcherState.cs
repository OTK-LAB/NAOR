using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArcherState : BaseState<Archer.State>
{
    protected GameObject playerGameObject;
    protected GameObject archerGameObject;
    protected Archer archerComponent;
    protected Rigidbody2D archerRigidBody;
    protected bool moveRight = false;
    
    public ArcherState(Archer.State key, GameObject _archerGameObject, GameObject _playerGameObject) : base(key)
    {
        archerGameObject = _archerGameObject;
        playerGameObject = _playerGameObject;

        archerComponent = archerGameObject.GetComponent<Archer>();
        archerRigidBody = archerComponent.GetComponent<Rigidbody2D>();
    }
}
