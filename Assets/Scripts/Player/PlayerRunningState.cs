using UnityEngine;

public class PlayerRunningState : PlayerBaseState
{
    public PlayerRunningState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {}
    public override void EnterState()
    {
        if(SuperState == Factory.Jump()){
            //Debug.Log("run : Jump");
        }
        if(SuperState==Factory.Slide())
        {
            //Debug.Log("run : slide");
        }
    }
    public override void UpdateState()
    {
        if(SuperState == Factory.Grounded())
        {
            Ctx.PlayerAnimator.Play("PlayerRun");
            
        }
        if(SuperState == Factory.Crouch()){
            Ctx.PlayerAnimator.Play("PlayerCrawl");
            Ctx.AppliedMovement = Ctx.CurrentMovementInput.x * (Ctx.MovementSpeed / 2);
        }else if(SuperState == Factory.Slide())
        {
            Ctx.AppliedMovement = 0;
        }
        else
        {
            Ctx.AppliedMovement = Ctx.CurrentMovementInput.x * Ctx.MovementSpeed;
        }
        CheckSwitchStates();
    }
    public override void ExitState()
    {

    }
    public override void CheckSwitchStates()
    {
        if(!Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }
        if(Ctx.IsOnSlope)
        {
            SwitchState(Factory.Slide());
        }
    }
    public override void InitializeSubstate()
    {

    }

    public override string StateName()
    {
        return "Run";
    }
}
