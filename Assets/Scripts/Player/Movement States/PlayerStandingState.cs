using UnityEngine;

public class PlayerStandingState : PlayerBaseState
{
    public PlayerStandingState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {}
    public override void EnterState()
    {
        InitializeSubstate();
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
        if(Ctx.IsOnSlope)
        {
            SwitchState(Factory.Slide());
        }
        if(Ctx.DragToggle)
        {
            SwitchState(Factory.Drag());
        }
        if(Ctx.IsCrouching)
        {
            SwitchState(Factory.Crouch());
        }
        if(Ctx.CanClimbLedge)
        {
            SwitchState(Factory.Climb());
        }
        if(Ctx.IsJumpPressed)
        {
            SwitchState(Factory.Jump());
        }
        if (Ctx.IsDashPressed && Ctx.CanDash)
        {
            SwitchState(Factory.Dash());
        }

    }
    public override void InitializeSubstate()
    {
        if(Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Run());
        }
        if(!Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Idle());
        }
    }
}
