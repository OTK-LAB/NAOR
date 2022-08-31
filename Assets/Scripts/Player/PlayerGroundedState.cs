using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    public override void EnterState()
    {
        InitializeSubstate();
        //Debug.Log("GROUNDED");
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
        if(!Ctx.IsOnGround)
        {
            SwitchState(Factory.InAir());
        }
    }
    public override void InitializeSubstate()
    {
        if(Ctx.IsOnSlope)
        {
            SetSubState(Factory.Slide());
        }
        else
        {
            SetSubState(Factory.Standing());
        }
    }
}