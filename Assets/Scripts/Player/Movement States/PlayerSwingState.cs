using UnityEngine;

public class PlayerSwingState : PlayerBaseState
{
    public PlayerSwingState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        InitializeSubstate();
        Ctx.CanDetectSwing = false;
        Ctx.CanSwing = false;
        Ctx.Rigidbod.gravityScale = 3;
        Ctx.Rigidbod.constraints = RigidbodyConstraints2D.None;
        Ctx.CanFlip = false;
        Ctx.CanMove = false;
        Ctx.GetComponent<HingeJoint2D>().enabled = true;
        Ctx.PlayerAnimator.Play("PlayerSwing");
        //Ctx.transform.position = Ctx.TopRaycastHit.collider.transform.position + (Ctx.FacingRight ? new Vector3(-0.39f, 0.13f): new Vector3(0.39f, -0.13f));
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.Rigidbod.gravityScale = Ctx.DefaultGravity;
        Ctx.GetComponent<HingeJoint2D>().enabled = false;
    }

    public override void CheckSwitchStates()
    {
        if(Ctx.IsJumpPressed)
        {
            SwitchState(Factory.SwingJump());
        }
    }

    public override void InitializeSubstate()
    {
        SetSubState(Factory.Idle());
    }
}
