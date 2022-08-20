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
        Debug.Log("GROUNDED");
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
        if(Ctx.IsCrouching)
        {
            SwitchState(Factory.Crouch());
        }
        if(Ctx.DragToggle)
        {
            SwitchState(Factory.Drag());
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

    public override string StateName()
    {
        return "Grounded";
    }
}