using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchState : State, IMove1D
{
    enum Phase { SpeedUp, SlowDown, Null }
    Phase phase = Phase.Null;
    private float curveTime;
    public PlayerCrouchState(Ultimate2DPlayer player, PlayerStateMachine stateMachine, Ultimate2DPlayer.StateName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
    {
    }

    public override void Enter()
    {
        base.Enter();
        rigidbody2D.gravityScale = playerData.Walk.Physics2DGravityScale;
        localTime = 0f;
        curveTime = SetCurveTimeByValue(playerData.Crouch.SpeedUpCurve, rigidbody2D.velocity.x / playerData.Crouch.MaxSpeed, playerData.Crouch.SpeedUpTime, true);
        phase = Phase.Null;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Move1D();
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
        if (!inputManager.Input_Crouch)
        {
            if (inputManager.Input_Walk == 0f)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else if (inputManager.Input_Walk != 0)
            {
                stateMachine.ChangeState(player.WalkState);
            }
            
        }
        else if (!playerData.Check.IsGrounded)
        {
            stateMachine.ChangeState(player.LandState);
        }
        else if (inputManager.Input_Dash)
        {
            stateMachine.ChangeState(player.DashState);
        }
    }

    public void Move1D()
    {
        float newVelocity;

        bool moveOnSlope = playerData.Check.IsOnSlope && playerData.Check.CurrentSlopeAngle < playerData.Check.MaxSlopeAngle;
        if (moveOnSlope)
        {
            newVelocity = VelocityOnX();
            rigidbody2D.velocity = -1 * newVelocity * playerData.Check.OnSlopeSpeedDirection.normalized;
            Debug.DrawRay(player.transform.position, rigidbody2D.velocity.normalized);
        }
        else
        {
            newVelocity = VelocityOnX();
            rigidbody2D.velocity = new Vector2(newVelocity, rigidbody2D.velocity.y);
        }
    }
    private float VelocityOnX()
    {
        float XVelocity;
        if (inputManager.Input_Walk != 0)
        {
            if (phase != Phase.SpeedUp)
            {
                curveTime = SetCurveTimeByValue(playerData.Crouch.SpeedUpCurve, Mathf.Abs(rigidbody2D.velocity.x) / playerData.Crouch.MaxSpeed, playerData.Crouch.SpeedUpTime, true);
                phase = Phase.SpeedUp;
            }
            if (curveTime < playerData.Crouch.SpeedUpTime)
            {
                XVelocity = playerData.Crouch.SpeedUpCurve.Evaluate(curveTime / playerData.Crouch.SpeedUpTime);
            }
            else
            {
                XVelocity = playerData.Crouch.SpeedUpCurve.Evaluate(1f);
            }
            XVelocity *= inputManager.Input_Walk * playerData.Crouch.MaxSpeed;
        }
        else
        {
            if (phase != Phase.SlowDown)
            {
                curveTime = SetCurveTimeByValue(playerData.Crouch.SlowDownCurve, Mathf.Abs(rigidbody2D.velocity.x) / playerData.Crouch.MaxSpeed, playerData.Crouch.SlowDownTime, false);
                phase = Phase.SlowDown;
            }
            if (curveTime < playerData.Crouch.SlowDownTime)
            {
                XVelocity = playerData.Crouch.SlowDownCurve.Evaluate(curveTime / playerData.Crouch.SlowDownTime);
            }
            else
            {
                XVelocity = playerData.Crouch.SlowDownCurve.Evaluate(1f);
            }
            XVelocity *= playerData.Crouch.MaxSpeed * player.transform.localScale.x;
        }
        return XVelocity;
    }
}
