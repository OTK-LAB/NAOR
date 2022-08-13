using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubstate();
    }
    public override void EnterState()
    {
        Debug.Log("Welcome to the grounded state!");
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
        if(Ctx.IsJumpPressed)
        {
            SwitchState(Factory.Jump());
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