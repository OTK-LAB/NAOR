using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {}
    public override void EnterState()
    {
        if(SuperState == Factory.Jump()){
            //Debug.Log("idle : Jump");
        }
        if(SuperState==Factory.Slide())
        {
            //Debug.Log("idle : slide");
        }
        Ctx.AppliedMovement = 0;

    }
    public override void UpdateState()
    {
        if(SuperState == Factory.Grounded())
        {
            Ctx.PlayerAnimator.Play("PlayerIdle");
        }
        if(SuperState == Factory.Crouch())
        {
            Ctx.PlayerAnimator.Play("PlayerCrouch");
        }

        CheckSwitchStates();
    }
    public override void ExitState()
    {

    }
    public override void CheckSwitchStates()
    {
        if(Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Run());
        }
        if(Ctx.IsOnSlope)
        {   
            SwitchState(Factory.Slide());
        }
        //if(Ctx.CanClimbLedge)
        //{
        //    if(SuperState == Factory.Grounded() && Ctx.IsJumpPressed){
        //        SwitchState(Factory.Climb());
        //    }
        //    if(SuperState == Factory.Jump())
        //    {   
        //        Debug.Log("buraya kadar celdum");
        //        SwitchState(Factory.Hang());
        //    }
        //}

    }
    public override void InitializeSubstate()
    {

    }

    public override string StateName()
    {
        return "Idle";
    }
}
