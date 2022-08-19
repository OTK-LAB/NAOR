using UnityEngine;

public class PlayerDragState : PlayerBaseState
{
    //Used a workaround solution to detach the object we drag
    Transform groundcheck;
    Transform frontcheck;

    public PlayerDragState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubstate();
    }
    public override void EnterState()
    {
        Debug.Log("DRAGGING");
        Ctx.Ray.transform.SetParent(Ctx.Rigidbod.transform);
        groundcheck = Ctx.GroundCheck;
        frontcheck = Ctx.FrontCheck;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        Ctx.Rigidbod.transform.DetachChildren();
        groundcheck.SetParent(Ctx.Rigidbod.transform);
        frontcheck.SetParent(Ctx.Rigidbod.transform);
    }
    public override void CheckSwitchStates()
    {
        if(!Ctx.DragToggle)
        {
            SwitchState(Factory.Grounded());
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