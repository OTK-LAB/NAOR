using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class PlayerJumpState : State, IMove2D
{
    float curveTime;
    enum Phase{ SpeedUp, SlowDown, Null}
    Phase phase = Phase.Null;
    public PlayerJumpState(Ultimate2DPlayer player, PlayerStateMachine stateMachine, Ultimate2DPlayer.StateName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.Rigidbody2D.velocity = new Vector2(player.Rigidbody2D.velocity.x, playerData.Jump.MaxHeight);
        playerData.Jump.JumpBufferTimer = 0;
        rigidbody2D.gravityScale = playerData.Jump.Physics2DGravityScale;
        curveTime = SetCurveTimeByValue(playerData.Jump.XSpeedUpCurve, rigidbody2D.velocity.x / playerData.Walk.MaxSpeed, playerData.Jump.XSpeedUpTime, true);
        phase = Phase.Null;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Move2D();
        curveTime += Time.fixedDeltaTime;
    }

    public override void Exit()
    {
        base.Exit();
        playerData.Check.CutJump = false;
    }

    public override void PhysicsCheck()
    {
        base.PhysicsCheck();
    }

    public override void SwitchStateLogic()
    {
        base.SwitchStateLogic();

        if (rigidbody2D.velocity.y <= 0)
        {
            stateMachine.ChangeState(player.LandState);
        }
        else if (inputManager.Input_Dash)
        {
            stateMachine.ChangeState(player.DashState);
        }
        else if (playerData.Check.CanPlungeAttack && inputManager.Input_PlungeAttack)
        {
            stateMachine.ChangeState(player.PlungeAttackDiveState);
        }
    }

    public void Move2D()
    {
        Vector2 newVelocity = Vector2.zero;
        if (!(localTime > playerData.Jump.JumpTime))
        {
            newVelocity.y = playerData.Jump.JumpVelocityCurve.Evaluate(localTime / playerData.Jump.JumpTime);
            newVelocity.y *= playerData.Jump.MaxHeight * 1 / playerData.Jump.JumpTime;
        }
        else
        {
            stateMachine.ChangeState(player.LandState);
        }
        if (playerData.Check.CutJump && newVelocity.y > 0)
        {
            //revision for jumpcut
            //newVelocity.y *= 1 - playerData.Jump.JumpCutPower;
        }
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
                curveTime = SetCurveTimeByValue(playerData.Jump.XSpeedUpCurve, Mathf.Abs(rigidbody2D.velocity.x) / playerData.Jump.XMaxSpeed, playerData.Jump.XSpeedUpTime, true);
                phase = Phase.SpeedUp;
            }
            if (curveTime < playerData.Jump.XSpeedUpTime)
            {
                XVelocity = playerData.Jump.XSpeedUpCurve.Evaluate(curveTime / playerData.Jump.XSpeedUpTime);
            }
            else
            {
                XVelocity = playerData.Jump.XSpeedUpCurve.Evaluate(1f);
            }
            XVelocity *= inputManager.Input_Walk * playerData.Jump.XMaxSpeed;
        }
        else
        {
            if (phase != Phase.SlowDown)
            {
                curveTime = SetCurveTimeByValue(playerData.Jump.XSlowDownCurve, Mathf.Abs(rigidbody2D.velocity.x) / playerData.Jump.XMaxSpeed, playerData.Jump.XSlowDownTime, false);
                phase = Phase.SlowDown;
            }
            if (curveTime < playerData.Jump.XSlowDownTime)
            {
                XVelocity = playerData.Jump.XSlowDownCurve.Evaluate(curveTime / playerData.Jump.XSlowDownTime);
            }
            else
            {
                XVelocity = playerData.Jump.XSlowDownCurve.Evaluate(1f);
            }
            XVelocity *= playerData.Jump.XMaxSpeed * player.transform.localScale.x;
        }
        return XVelocity;
    }
    
}
