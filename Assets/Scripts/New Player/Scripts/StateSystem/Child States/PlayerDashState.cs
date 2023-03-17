using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : State, IMove2D
{
    public PlayerDashState(Ultimate2DPlayer player, PlayerStateMachine stateMachine, Ultimate2DPlayer.StateName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
    {
    }

    public override void Enter()
    {
        base.Enter();
        rigidbody2D.gravityScale = playerData.Dash.Physics2DGravityScale;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Move2D();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void PhysicsCheck()
    {
        base.PhysicsCheck();
    }

    public override void SwitchStateLogic()
    {
        base.SwitchStateLogic();
        if(localTime > playerData.Dash.DashTime)
        {
            if(inputManager.Input_Walk != 0 && playerData.Check.IsGrounded)
            {
                stateMachine.ChangeState(player.WalkState);
            }
            else if (rigidbody2D.velocity.y == 0 && inputManager.Input_Walk == 0 && playerData.Check.IsGrounded && (!playerData.Check.IsOnSlope || playerData.Check.IsOnMovableSlope))
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else
            {
                if (playerData.Check.CanJump && inputManager.Input_Jump)
                {
                    stateMachine.ChangeState(player.JumpState);
                }
                else
                {
                    stateMachine.ChangeState(player.LandState);
                }
            }
        }
    }

    public void Move2D()
    {
        Vector2 XVelocity = Vector2.zero;
        XVelocity.x = playerData.Dash.DashXVelocityCurve.Evaluate(localTime / playerData.Dash.DashTime);
        XVelocity.x *= playerData.Dash.MaxSpeed * player.transform.localScale.x;
        XVelocity.y = playerData.Dash.DashYVelocityCurve.Evaluate(localTime / playerData.Dash.DashTime);
        XVelocity.y *= playerData.Dash.MaxHeight;

        rigidbody2D.velocity = XVelocity;
    }
}
