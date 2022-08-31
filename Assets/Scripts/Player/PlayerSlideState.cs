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
        InitializeSubstate();
        Ctx.CanFlip = false;
        Ctx.Rigidbod.velocity = Vector2.zero;
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
        Ctx.CanFlip = true;
    }
    public override void CheckSwitchStates()
    {
        if(!Ctx.IsOnSlope)
        {
            SwitchState(Factory.Standing());
        }
    }
    public override void InitializeSubstate()
    {
        SetSubState(Factory.Idle());
    }
}
