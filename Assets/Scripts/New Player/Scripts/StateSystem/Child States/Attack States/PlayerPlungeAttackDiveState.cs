using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlungeAttackDiveState : AttackState, IMove1D
{
    public PlayerPlungeAttackDiveState(Ultimate2DPlayer player, PlayerStateMachine stateMachine, Ultimate2DPlayer.StateName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
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
        Move1D();
    }

    public override void PhysicsCheck()
    {
        base.PhysicsCheck();
    }

    public override void SwitchStateLogic()
    {
        base.SwitchStateLogic();
        if(playerData.Check.IsGrounded)
        {
            stateMachine.ChangeState(player.PlungeAttackLandState);
        }
    }

    public override void Update()
    {
        base.Update();
    }

    public void Move1D()
    {
        float _velocity = 0f;
        float _curveTime = localTime - playerData.AttackState.PlungeAttack.WaitDuration;
        if (localTime < playerData.AttackState.PlungeAttack.WaitDuration)
        {
            if (localTime > playerData.AttackState.PlungeAttack.SpeedUpTime)
            {
                _velocity = playerData.AttackState.PlungeAttack.SpeedUpCurve.Evaluate(_curveTime / playerData.AttackState.PlungeAttack.SpeedUpTime);
            }
            else
            {
                _velocity = playerData.AttackState.PlungeAttack.SpeedUpCurve.Evaluate(1f);
            }
        }
        _velocity *= playerData.AttackState.PlungeAttack.MinYVelocity;
        rigidbody2D.velocity = new Vector2(0, _velocity);
    }
}
