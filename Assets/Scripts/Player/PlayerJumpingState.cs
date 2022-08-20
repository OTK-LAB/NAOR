using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
    public PlayerJumpingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {
        IsRootState = true;
    }
    public override void EnterState()
    {
        InitializeSubstate();
        Debug.Log("JUMP STATE");
        Ctx.Rigidbod.velocity = new Vector2(Ctx.Rigidbod.velocity.x, Ctx.JumpForce);
        Ctx.PlayerAnimator.Play("PlayerJump");
    }
    public override void UpdateState()
    {   
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        //FIXME:
        // Does not work for now
        Ctx.PlayerAnimator.Play("PlayerLand");
    }
    public override void CheckSwitchStates()
    {
        if(Ctx.IsOnGround)
        {
            SwitchState(Factory.Grounded());
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

    public override string StateName()
    {
        return "Jump";
    }
}