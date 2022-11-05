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
        CheckSwitchStates();
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
            Debug.Log("RUN update");
        }
        Debug.Log("Run çalışıyor");
    }
    public override void ExitState()
    {
        Debug.Log("RUN ÇIKTI");
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
