using Unity.VisualScripting;
using System.Collections;
using UnityEngine;


public class PlayerStandingState : PlayerBaseState
{
    public PlayerStandingState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {}

    

    public override void EnterState()
    {
        if (Ctx.GroundCollider != null)
        {
            Physics2D.IgnoreCollision(Ctx.GroundCollider, Ctx.PlayerCollider, false);
        }
        InitializeSubstate();
    }
    public override void UpdateState()
    {
        if(Ctx.GroundCollider.gameObject.CompareTag("DowningPlatform") && Ctx.IsDownPressed)
        {
            Physics2D.IgnoreCollision(Ctx.GroundCollider, Ctx.PlayerCollider, true);
            
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
