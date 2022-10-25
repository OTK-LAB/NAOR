using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
    public PlayerJumpingState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {}
    public override void EnterState()
    {
        InitializeSubstate();
        Ctx.Rigidbod.velocity = new Vector2(Ctx.Rigidbod.velocity.x, Ctx.JumpForce);
        Ctx.PlayerAnimator.Play("PlayerJump");        
    }
    public override void UpdateState()
    {   
        CheckSwitchStates();
    }
    public override void ExitState()
    {

    }
    public override void CheckSwitchStates()
    {
        if(Ctx.Rigidbod.velocity.y < 0)
        {
            SwitchState(Factory.Fall());
        }
        /*if(Ctx.CanClimbLedge)
        {
            SwitchState(Factory.Hang());
        }*/
        if(Ctx.CanSwing)
        {
            SwitchState(Factory.Swing());
        }
    }
    public override void InitializeSubstate()
    {
        if(!Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Idle());
        }
        if(Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Run());
        }
    }
}