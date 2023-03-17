using UnityEngine;

public class Old_PlayerJumpingState : Old_PlayerBaseState
{
    public Old_PlayerJumpingState(PlayerController currentContext, Old_PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {}
    public override void EnterState()
    {
        InitializeSubstate();
        Ctx.Rigidbod.velocity = new Vector2(Ctx.Rigidbod.velocity.x, Ctx.JumpForce);
        Ctx.PlayerAnimator.Play("PlayerJump");        
        //This line causes jumping again when jump pressed while falling (?)
        Ctx.IsJumpPressed = false;
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