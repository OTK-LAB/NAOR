using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    public PlayerDashState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { 
        IsRootState = true;
    }
    static float currentDashingTime;
    public override void EnterState()
    {
        Ctx.Rigidbod.velocity = new Vector2(0, 0);
        Debug.Log("In Enter");
        Ctx.CanFlip = false;
        Ctx.CanDash = false;
        currentDashingTime = Ctx.DashingTime;
        Ctx.PlayerAnimator.Play("PlayerDash");
        Ctx.GetComponent<SpriteRenderer>().color = new Color(Ctx.GetComponent<SpriteRenderer>().color.r, Ctx.GetComponent<SpriteRenderer>().color.g, Ctx.GetComponent<SpriteRenderer>().color.b, 135/255f);
        Ctx.Rigidbod.gravityScale = 0f;
        Ctx.GetComponent<HealthSystem>().Invincible = true;
    }
    public override void UpdateState()
    {
        Debug.Log("In Update");
            if (Ctx.FacingRight == true)
            {
                Ctx.AppliedMovement = Ctx.DashingVelcoity;
                currentDashingTime -= Time.deltaTime;

            }
            else if (Ctx.FacingRight == false)
            {
                Ctx.AppliedMovement = -Ctx.DashingVelcoity;
                currentDashingTime -= Time.deltaTime;
            }
            if (Ctx.ThereIsGroundFront && (Ctx.Ray.collider.CompareTag("Passable")))
            {

                Ctx.PlayerCollider.isTrigger = true;
            }
            else
            {
                Ctx.PlayerCollider.isTrigger = false;
            }
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        Debug.Log("In Exit");
        Ctx.Rigidbod.gravityScale = Ctx.DefaultGravity;
        Ctx.CanFlip = true;
        Ctx.CanDash = true;
        Ctx.PlayerCollider.isTrigger = false;
        Ctx.GetComponent<HealthSystem>().Invincible = false;
        Ctx.GetComponent<SpriteRenderer>().color = new Color(Ctx.GetComponent<SpriteRenderer>().color.r, Ctx.GetComponent<SpriteRenderer>().color.g, Ctx.GetComponent<SpriteRenderer>().color.b, 1);
        Ctx.IsDashPressed = false;
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
