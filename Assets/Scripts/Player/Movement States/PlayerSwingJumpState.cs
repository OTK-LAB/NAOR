using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwingJumpState : PlayerBaseState
{
    public PlayerSwingJumpState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    //rb rotationý yavaþ yavaþ azalt, x inputu almasýn, 
    public override void EnterState()
    {
        Ctx.Rigidbod.velocity = new Vector2((Ctx.Rigidbod.rotation / 8) + Ctx.Rigidbod.velocity.x, Ctx.JumpForce);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
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
