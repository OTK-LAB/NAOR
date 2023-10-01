using System;
using UltimateCC;
using UnityEngine;

public class AttackState : MainState
{
    protected float attackDuration;
    protected float maxStateTime;

    public static event Action OnEnemyKilled;

    public AttackState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        RaycastHit2D hit2D = Physics2D.Raycast(player.transform.position,new Vector2(playerData.Physics.FacingDirection,0), 2f, playerData.Physics.EnemyLayerMask);
        if (hit2D)
        {
            MonoBehaviour.Destroy(hit2D.collider.gameObject);
            OnEnemyKilled?.Invoke();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void PhysicsCheck()
    {
        base.PhysicsCheck();
    }

    public override void SwitchStateLogic()
    {
        base.SwitchStateLogic();
    }
}
