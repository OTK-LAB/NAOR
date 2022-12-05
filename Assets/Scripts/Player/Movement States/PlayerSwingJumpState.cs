using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwingJumpState : PlayerBaseState
{
    public PlayerSwingJumpState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
 
    public override void EnterState()
    {
        Ctx.Rigidbod.velocity = new Vector2((Ctx.Rigidbod.rotation / 20) + Ctx.Rigidbod.velocity.x, Ctx.JumpForce * 1.15f);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        //Ctx.Rigidbod.rotation = Mathf.Lerp(Ctx.Rigidbod.rotation, 0, Time.deltaTime * 4);
        if (Ctx.Rigidbod.rotation < 0)
            Ctx.Rigidbod.rotation += 1f;
        if (Ctx.Rigidbod.rotation > 0)
            Ctx.Rigidbod.rotation -= 1f;
        if (Mathf.Abs(Ctx.Rigidbod.rotation) < 0.1f)
            Ctx.Rigidbod.rotation = 0;
    }

    public override void ExitState()
    {
        Ctx.CanDetectSwing = true;
        Ctx.CanFlip = true;
        Ctx.CanMove = true;
        Ctx.Rigidbod.rotation = 0;
        Ctx.Rigidbod.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.Rigidbod.velocity.y < 0)
        {
            SwitchState(Factory.Fall());
        }
        if (Ctx.CanSwing)
        {
            SwitchState(Factory.Swing());
        }
    }

    public override void InitializeSubstate()
    {

    }
}
