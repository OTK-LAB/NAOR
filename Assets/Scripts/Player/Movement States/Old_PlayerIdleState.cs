using UnityEngine;

public class Old_PlayerIdleState : Old_PlayerBaseState
{
    public Old_PlayerIdleState(PlayerController currentContext, Old_PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {}
    public override void EnterState()
    {
        Ctx.AppliedMovement = 0;

        if(SuperState == Factory.Standing())
        {
            Ctx.PlayerAnimator.Play("PlayerIdle");
        }
        if(SuperState == Factory.Crouch())
        {
            Ctx.PlayerAnimator.Play("PlayerCrouch");
        }
    }
    public override void UpdateState()
    {
        
        //TODO: 
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
    }
    public override void InitializeSubstate()
    {

    }
}
