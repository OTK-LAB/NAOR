using Unity.VisualScripting;
using System.Collections;
using UnityEngine;


public class Old_PlayerStandingState : Old_PlayerBaseState
{
    public Old_PlayerStandingState(PlayerController currentContext, Old_PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {}


    Collider2D tempcol;
    public override void EnterState()
    {
        if (tempcol!=null)
        {
            Physics2D.IgnoreCollision(tempcol, Ctx.PlayerCollider, false);
        }
        InitializeSubstate();
    }
    public override void UpdateState()
    {
        if(Ctx.GroundCollider.gameObject.CompareTag("Droppable") && Ctx.IsDownPressed)
        {
            tempcol = Ctx.GroundCollider;
            Physics2D.IgnoreCollision(Ctx.GroundCollider, Ctx.PlayerCollider, true);
            
        }

        CheckSwitchStates();
    }
    public override void ExitState()
    {
      
    }
    public override void CheckSwitchStates()
    {
        if(Ctx.IsCrouching)
        {
            SwitchState(Factory.Crouch());
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
