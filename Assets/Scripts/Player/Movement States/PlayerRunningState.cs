using UnityEngine;

public class PlayerRunningState : PlayerBaseState
{
    public PlayerRunningState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {}
    public override void EnterState()
    {

    }
    public override void UpdateState()
    {
        if(SuperState == Factory.Standing())
        {
            Ctx.PlayerAnimator.Play("PlayerRun");
            
        }
        if(SuperState == Factory.Crouch()){
            Ctx.PlayerAnimator.Play("PlayerCrawl");
            Ctx.AppliedMovement = Ctx.CurrentMovementInput.x * (Ctx.MovementSpeed / 2);
        }
        else if(SuperState == Factory.Slide())
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
    }
    public override void InitializeSubstate()
    {

    }
}
