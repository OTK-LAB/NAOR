using UnityEngine;

public class PlayerIdleState : State
{
    public PlayerIdleState(Ultimate2DPlayer player, PlayerStateMachine stateMachine, Ultimate2DPlayer.StateName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
    {
    }

    public override void Enter()
    {
        base.Enter();
        rigidbody2D.gravityScale = playerData.Walk.Physics2DGravityScale;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        rigidbody2D.velocity = Vector2.zero;
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

        if (inputManager.Input_Walk != 0)
        {
            stateMachine.ChangeState(player.WalkState);
        }
        else if (inputManager.Input_Jump && playerData.Check.CanJump)
        {
            stateMachine.ChangeState(player.JumpState);
        }
        else if (!playerData.Check.IsGrounded && rigidbody2D.velocity.y <= 0)
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
}
