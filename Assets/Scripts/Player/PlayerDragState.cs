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
    }
    public override void EnterState()
    {
        InitializeSubstate();
        Debug.Log("DRAGGING");
        Ctx.Ray.transform.SetParent(Ctx.Rigidbod.transform);
        Ctx.CanFlip = false;
        groundcheck = Ctx.GroundCheck;
        frontcheck = Ctx.FrontCheck;
        //FIXME:
        //This is going to be controlled in idle and movement states once we have proper animations
        Ctx.PlayerAnimator.Play("PlayerDrag");
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
        Ctx.CanFlip = true;
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
    public override string StateName()
    {
        return "Drag";
    }
}