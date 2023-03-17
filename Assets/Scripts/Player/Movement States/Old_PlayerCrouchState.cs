using UnityEngine;

public class Old_PlayerCrouchState : Old_PlayerBaseState
{
    public Old_PlayerCrouchState(PlayerController currentContext, Old_PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory)
    {}
    public override void EnterState()
    {
        InitializeSubstate();
        //Debug.Log("CROUCHING");
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {

    }
    public override void CheckSwitchStates()
    {
        if(!Ctx.IsCrouching)
        {
            SwitchState(Factory.Standing());
        }
    }
    public override void InitializeSubstate()
    {
        if(!Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Idle());
        }
        if(Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Run());
        }
    }
}