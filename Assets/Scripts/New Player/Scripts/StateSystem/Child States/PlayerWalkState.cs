using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerWalkState : State, IMove1D
{
    enum Phase{ SpeedUp, SlowDown, Null}
    Phase phase = Phase.Null;
    private float curveTime;
    public PlayerWalkState(Ultimate2DPlayer player, PlayerStateMachine stateMachine, Ultimate2DPlayer.StateName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
    {
    }

    public override void Enter()
    {
        base.Enter();
        rigidbody2D.gravityScale = playerData.Walk.Physics2DGravityScale;
        localTime = 0f;
        curveTime = SetCurveTimeByValue(playerData.Walk.SpeedUpCurve, rigidbody2D.velocity.x / playerData.Walk.MaxSpeed, playerData.Walk.SpeedUpTime, true);
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
        if (inputManager.Input_Walk == 0f && rigidbody2D.velocity.x == 0f)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        else if (inputManager.Input_Jump && playerData.Check.CanJump)
        {
            stateMachine.ChangeState(player.JumpState);
        }
        else if (!playerData.Check.IsGrounded)
        {
            stateMachine.ChangeState(player.LandState);
        }
        else if (inputManager.Input_Dash)
        {
            stateMachine.ChangeState(player.DashState);
        }
        else if (inputManager.Input_Crouch)
        {
            stateMachine.ChangeState(player.CrouchState);
        }
    }

    public void Move1D()
    {
        float newVelocity;

        if (playerData.Check.IsOnMovableSlope)
        {
            newVelocity = VelocityOnX();
            rigidbody2D.velocity = -1 * newVelocity * playerData.Check.OnSlopeSpeedDirection.normalized;
            Debug.DrawRay(player.transform.position, rigidbody2D.velocity.normalized);
        }
        else if(!playerData.Check.IsOnMovableSlope && playerData.Check.IsOnSlope && Mathf.Sign(playerData.Check.SlopeContactPosition.x - player.transform.position.x) == Mathf.Sign(player.transform.localScale.x))
        {
            rigidbody2D.velocity = Vector2.zero;
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
            if(phase != Phase.SpeedUp)
            {
                curveTime = SetCurveTimeByValue(playerData.Walk.SpeedUpCurve, Mathf.Abs(rigidbody2D.velocity.x) / playerData.Walk.MaxSpeed, playerData.Walk.SpeedUpTime, true);
                phase = Phase.SpeedUp;
            }
            if (curveTime < playerData.Walk.SpeedUpTime)
            {
                XVelocity = playerData.Walk.SpeedUpCurve.Evaluate(curveTime / playerData.Walk.SpeedUpTime);
            }
            else
            {
                XVelocity = playerData.Walk.SpeedUpCurve.Evaluate(1f);
            }
            XVelocity *= inputManager.Input_Walk * playerData.Walk.MaxSpeed;
        }
        else
        {
            if (phase != Phase.SlowDown)
            {
                curveTime = SetCurveTimeByValue(playerData.Walk.SlowDownCurve, Mathf.Abs(rigidbody2D.velocity.x) / playerData.Walk.MaxSpeed, playerData.Walk.SlowDownTime, false);
                phase = Phase.SlowDown;
            }
            if (curveTime < playerData.Walk.SlowDownTime)
            {
                XVelocity = playerData.Walk.SlowDownCurve.Evaluate(curveTime / playerData.Walk.SlowDownTime);
            }
            else
            {
                XVelocity = playerData.Walk.SlowDownCurve.Evaluate(1f);
            }
            XVelocity *= playerData.Walk.MaxSpeed * player.transform.localScale.x;
        }
        return XVelocity;
    }
}
