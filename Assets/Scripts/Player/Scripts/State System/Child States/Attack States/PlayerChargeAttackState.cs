using UltimateCC;
using UnityEngine;

public class PlayerChargeAttackState : AttackState
{
    public PlayerChargeAttackState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
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
        playerData.Attack.AttackColliders.Find(x => x.Type == PlayerData.AttackStateVariables.AttackType.Heavy).Collider.enabled = false;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        rigidbody2D.velocity = new(0,0);
        base.FixedUpdate();
    }

    public override void PhysicsCheck()
    {
        if (localTime > playerData.Attack.ChargeAttack.ChargeTimeMaxTime)
        {
            playerData.Attack.AttackColliders.Find(x => x.Type == PlayerData.AttackStateVariables.AttackType.Heavy).Collider.enabled = true;
        }
    }

    public override void SwitchStateLogic()
    {
        base.SwitchStateLogic();
        if (localTime < playerData.Attack.ChargeAttack.ChargeTimeMaxTime && !inputManager.Input_Attack)
        {
            stateMachine.ChangeState(player.BasicAttack1State);
        }
        else if (inputManager.Input_Walk != 0 && !inputManager.Input_Attack)
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
            stateMachine.ChangeState(player.CrouchIdleState);
        }
        else if (localTime > maxStateTime)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
