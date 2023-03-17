using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Old_PlayerDashState : Old_PlayerBaseState
{
    public Old_PlayerDashState(PlayerController currentContext, Old_PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { 
        IsRootState = true;
    }
    static float currentDashingTime;
    public override void EnterState()
    {
        Ctx.Rigidbod.gravityScale = 0f;
        Ctx.Rigidbod.velocity = new Vector2(0, 0);
        Ctx.CanFlip = false;
        Ctx.CanDash = false;
        currentDashingTime = Ctx.DashingTime;
        Ctx.PlayerAnimator.Play("PlayerDash");
        Ctx.GetComponent<SpriteRenderer>().color = new Color(Ctx.GetComponent<SpriteRenderer>().color.r, Ctx.GetComponent<SpriteRenderer>().color.g, Ctx.GetComponent<SpriteRenderer>().color.b, 135/255f);
        Ctx.GetComponent<HealthSystem>().Invincible = true;
    }
    public override void UpdateState()
    {
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
            if (Ctx.DashPassCheck && (Ctx.DashRay.collider.CompareTag("Passable")))
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
        Ctx.Rigidbod.gravityScale = Ctx.DefaultGravity;
        Ctx.AppliedMovement = 0;
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
