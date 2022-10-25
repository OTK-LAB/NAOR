using UnityEngine;

public class PlayerSwingState : PlayerBaseState
{
    public PlayerSwingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        InitializeSubstate();
        Ctx.Rigidbod.gravityScale = 5;
        Ctx.Rigidbod.velocity = Vector2.zero;
        Ctx.Rigidbod.constraints = RigidbodyConstraints2D.None;
        Ctx.CanFlip = false;
        //Ctx.AppliedMovement = 0;
        Ctx.GetComponent<HingeJoint2D>().enabled = true;
        Ctx.PlayerAnimator.Play("PlayerSwing");
        Ctx.transform.position = Ctx.TopRaycastHit.collider.transform.position + new Vector3(-0.39f, 0.13f);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.CanFlip = true;
        Ctx.Rigidbod.gravityScale = Ctx.DefaultGravity;
        Ctx.transform.localRotation = Quaternion.Euler(Vector3.zero);
        Ctx.Rigidbod.constraints = RigidbodyConstraints2D.FreezeRotation;
        Ctx.GetComponent<HingeJoint2D>().enabled = false;
    }

    public override void CheckSwitchStates()
    {
        if(Ctx.IsJumpPressed)
        {
            SwitchState(Factory.Jump());
        }
    }

    public override void InitializeSubstate()
    {
        SetSubState(Factory.Idle());
    }
}
