using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerLandState : State, IMove2D
{
    float curveTime;
    enum Phase { SpeedUp, SlowDown, Null }
    Phase phase = Phase.Null;
    public PlayerLandState(Ultimate2DPlayer player, PlayerStateMachine stateMachine, Ultimate2DPlayer.StateName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
    {
    }

    public override void Enter()
    {
        base.Enter();
        rigidbody2D.gravityScale = playerData.Land.Physics2DGravityScale;
        curveTime = SetCurveTimeByValue(playerData.Land.XSpeedUpCurve, rigidbody2D.velocity.x / playerData.Walk.MaxSpeed, playerData.Land.XSpeedUpTime, true);
        phase = Phase.Null;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (playerData.Check.CutJump && rigidbody2D.velocity.y > 0)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y * playerData.Jump.JumpCutPower);
            playerData.Check.CutJump = false;
        }
        Move2D();
        curveTime += Time.fixedDeltaTime;
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

        if (rigidbody2D.velocity.y == 0 && playerData.Check.IsGrounded)
        {
            if (rigidbody2D.velocity.x != 0 && inputManager.Input_Walk != 0 && (!playerData.Check.IsOnSlope || playerData.Check.IsOnMovableSlope))
            {
                stateMachine.ChangeState(player.WalkState);
            }
            else if ((!playerData.Check.IsOnSlope || playerData.Check.IsOnMovableSlope))
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
        else if (inputManager.Input_Jump && playerData.Check.CanJump)
        {
            stateMachine.ChangeState(player.JumpState);
        }
        else if (playerData.Check.IsGrounded && playerData.Check.IsOnMovableSlope)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        else if (inputManager.Input_Dash)
        {
            stateMachine.ChangeState(player.DashState);
        }
    }

    public void Move2D()
    {
        Vector2 newVelocity = Vector2.zero;
        newVelocity.y = playerData.Land.LandVelocityCurve.Evaluate(localTime / playerData.Land.LandTime);
        newVelocity.y *= playerData.Jump.MaxHeight * 1 / playerData.Land.LandTime;
        newVelocity.y = Mathf.Clamp(newVelocity.y, playerData.Land.MinLandSpeed, float.MaxValue);

        if (!playerData.Check.IsOnSlope)
        {
            newVelocity.x = VelocityOnx();
        }
        rigidbody2D.velocity = newVelocity;
    }

    private float VelocityOnx()
    {
        float XVelocity;
        if (inputManager.Input_Walk != 0)
        {
            if (phase != Phase.SpeedUp)
            {
                curveTime = SetCurveTimeByValue(playerData.Land.XSpeedUpCurve, Mathf.Abs(rigidbody2D.velocity.x) / playerData.Land.MaxXSpeed, playerData.Land.XSpeedUpTime, true);
                phase = Phase.SpeedUp;
            }
            if (curveTime < playerData.Land.XSpeedUpTime)
            {
                XVelocity = playerData.Land.XSpeedUpCurve.Evaluate(curveTime / playerData.Land.XSpeedUpTime);
            }
            else
            {
                XVelocity = playerData.Land.XSpeedUpCurve.Evaluate(1f);
            }
            XVelocity *= inputManager.Input_Walk * playerData.Land.MaxXSpeed;
        }
        else
        {
            if (phase != Phase.SlowDown)
            {
                curveTime = SetCurveTimeByValue(playerData.Land.XSlowDownCurve, Mathf.Abs(rigidbody2D.velocity.x) / playerData.Land.MaxXSpeed, playerData.Land.XSlowDownTime, false);
                phase = Phase.SlowDown;
            }
            if (curveTime < playerData.Land.XSlowDownTime)
            {
                XVelocity = playerData.Land.XSlowDownCurve.Evaluate(curveTime / playerData.Land.XSlowDownTime);
            }
            else
            {
                XVelocity = playerData.Land.XSlowDownCurve.Evaluate(1f);
            }
            XVelocity *= playerData.Land.MaxXSpeed * player.transform.localScale.x;
        }
        return XVelocity;
    }
}
