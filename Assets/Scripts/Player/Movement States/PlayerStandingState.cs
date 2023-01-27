using Unity.VisualScripting;
using System.Collections;
using UnityEngine;


public class PlayerStandingState : PlayerBaseState
{
    public PlayerStandingState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {}

    private IEnumerator coroutine;

    public override void EnterState()
    {
        
        InitializeSubstate();
    }
    public override void UpdateState()
    {
        if(Ctx.canDown == true && Ctx.IsDownPressed)
        {
            Physics2D.IgnoreCollision(Ctx.dcol, Ctx.PlayerCollider, true);
            
        }
        else if (Ctx.canDown == false)
        {
            Physics2D.IgnoreCollision(Ctx.dcol, Ctx.PlayerCollider, false);
        }

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
        /*
        if (Ctx.IsDashPressed && Ctx.CanDash)
        {
            SwitchState(Factory.Dash());
        }
        */
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
