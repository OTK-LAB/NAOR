using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChargeAttackState : AttackState
{
    public PlayerChargeAttackState(Ultimate2DPlayer player, PlayerStateMachine stateMachine, Ultimate2DPlayer.StateName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
    {
    }

    public override void Enter()
    {
        base.Enter();
        attackDuration = playerData.Attack.ChargeAttack.AttackDuration;
        maxStateTime = playerData.Attack.ChargeAttack.MaxStateTime;
    }

    public override void Exit()
    {
        base.Exit();
        inputManager.Input_Attack = false;
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
        if (localTime < playerData.Attack.ChargeAttack.AttackDuration && !inputManager.Input_Attack)
        {
            stateMachine.ChangeState(player.BasicAttack1State);
        }
        else if (inputManager.Input_Walk != 0)
        {
            stateMachine.ChangeState(player.WalkState);
        }
        else if (inputManager.Input_Jump)
        {
            stateMachine.ChangeState(player.JumpState);
        }
        else if (inputManager.Input_Dash)
        {
            stateMachine.ChangeState(player.DashState);
        }
        else if (inputManager.Input_Crouch)
        {
           stateMachine.ChangeState(player.CrouchState);
        }
        else if (localTime > maxStateTime)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void Update()
    {
        base.Update();
    }
}
