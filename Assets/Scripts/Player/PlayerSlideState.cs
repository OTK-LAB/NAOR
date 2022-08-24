using UnityEngine;

public class PlayerSlideState : PlayerBaseState
{
    float gravity;
    public PlayerSlideState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {
        gravity = Ctx.Rigidbod.gravityScale;
    }
    public override void EnterState()
    {
        Ctx.AppliedMovement = 0;
        Ctx.Rigidbod.gravityScale = gravity * 5;
        //Debug.Log("SLIDE STATE");
        Ctx.PlayerAnimator.Play("PlayerSlide");
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        Ctx.Rigidbod.gravityScale = gravity;
    }
    public override void CheckSwitchStates()
    {
        if(!Ctx.IsOnSlope && !Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }
        else if(!Ctx.IsOnSlope && Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Run());
        }
    }
    public override void InitializeSubstate()
    {

    }

    public override string StateName()
    {
        return "Slide";
    }
}
