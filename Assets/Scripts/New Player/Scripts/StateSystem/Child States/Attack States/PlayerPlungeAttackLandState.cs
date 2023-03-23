using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlungeAttackLandState : AttackState
{
    public PlayerPlungeAttackLandState(Ultimate2DPlayer player, PlayerStateMachine stateMachine, Ultimate2DPlayer.StateName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
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

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void PhysicsCheck()
    {
        base.PhysicsCheck();
        if(localTime > 0.5f)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void SwitchStateLogic()
    {
        base.SwitchStateLogic();
    }

    public override void Update()
    {
        base.Update();
    }
}
