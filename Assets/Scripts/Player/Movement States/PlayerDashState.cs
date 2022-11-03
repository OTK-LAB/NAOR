using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerBaseState
{

    Vector2 positionAfterDash;
    public PlayerDashState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { 
        IsRootState = true;
    }
    static float currentDashingTime;
    Collider2D obstacle;
    public override void EnterState()
    {
        Ctx.Rigidbod.velocity = new Vector2(0, 0);
        Debug.Log("In Enter");
        Ctx.IsDashing = false;
        Ctx.CanFlip = false;
        Ctx.CanDash = false;
        currentDashingTime = Ctx.DashingTime;
        
        Ctx.Rigidbod.gravityScale = 0f;

    }
    public override void UpdateState()
    {
        Debug.Log("In Update");
        //   Ctx.Rigidbod.velocity = new Vector2(Ctx.das, Ctx.Rigidbod.velocity.y);
        if (!Ctx.IsDashing) { 
            if (Ctx.FacingRight == true)
            {
                //Ctx.Rigidbod.AddForce(new Vector2(Ctx.DashingVelcoity * 1, 0));
                Ctx.AppliedMovement = Ctx.DashingVelcoity;
                currentDashingTime = currentDashingTime - Time.deltaTime;

            }
            else if (Ctx.FacingRight == false)
            {
                //Ctx.Rigidbod.AddForce(new Vector2(Ctx.DashingVelcoity * -1, 0));
                Ctx.AppliedMovement = -Ctx.DashingVelcoity;
                currentDashingTime = currentDashingTime - Time.deltaTime;

            }
            if (Ctx.ThereIsGroundFront && (Ctx.Ray.collider.CompareTag("Passable")))
            {
                obstacle = Ctx.Ray.collider; //passableObjectCollider
                obstacle.isTrigger = true;
            }
            }
        CheckSwitchStates();


    }
    public override void ExitState()
    {
        Debug.Log("In Exit");
        Ctx.IsDashing = true;
        Ctx.Rigidbod.gravityScale = Ctx.DefaultGravity;
        Ctx.CanFlip = true;
        Ctx.CanDash = true;
        if(obstacle != null)
            obstacle.isTrigger = false;
    }
    public override void CheckSwitchStates()
    {
        if (currentDashingTime <= 0)
        {
            if (Ctx.IsOnGround)
            {
                SwitchState(Factory.Grounded());
            }
            else
            {
                SwitchState(Factory.InAir());
            }
        }
    }
    public override void InitializeSubstate()
    {
      
    }
}
