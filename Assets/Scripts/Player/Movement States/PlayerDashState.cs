using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    public PlayerDashState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    float originalDashingTime;
    float originalGravityScale;
    public override void EnterState()
    {
        InitializeSubstate();


        Debug.Log("In Enter");
        
        Ctx.CanDash = false;
        originalGravityScale = Ctx.Rigidbod.gravityScale;
        originalDashingTime = Ctx.DashingTime;

        Ctx.Rigidbod.gravityScale = 0f;

    }
    public override void UpdateState()
    {
        Debug.Log("In Update");
        //   Ctx.Rigidbod.velocity = new Vector2(Ctx.das, Ctx.Rigidbod.velocity.y);
        if (Ctx.FacingRight == true)
        {
            Ctx.Rigidbod.AddForce(new Vector2(Ctx.DashingVelcoity * 1, 0));
            Ctx.DashingTime = Ctx.DashingTime - Time.deltaTime;

        }
        else if (Ctx.FacingRight==false)
        {
            Ctx.Rigidbod.AddForce(new Vector2(Ctx.DashingVelcoity * -1, 0));
            Ctx.DashingTime = Ctx.DashingTime - Time.deltaTime;

        }

        CheckSwitchStates();
    }
    public override void ExitState()
    {
        Debug.Log("In Exit");
        Ctx.Rigidbod.gravityScale = originalGravityScale;
        Ctx.DashingTime = originalDashingTime;
        Ctx.CanDash = true;


    }
    public override void CheckSwitchStates()
    {
        if (Ctx.DashingTime<=0)
        {
            SwitchState(Factory.Standing());
        }
        if (Ctx.Rigidbod.velocity.y < 0)
        {
            SwitchState(Factory.Fall());
        }
        if (Ctx.CanClimbLedge)
        {
            SwitchState(Factory.Hang());
        }
    
    }
    public override void InitializeSubstate()
    {
        if (!Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Idle());
        }
        if (Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Run());
        }
    }
}
