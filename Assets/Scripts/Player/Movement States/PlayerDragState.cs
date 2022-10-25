using UnityEngine;

public class PlayerDragState : PlayerBaseState
{
    //Used a workaround solution to detach the object we drag
    Transform groundcheck;
    Transform frontcheck;
    Transform topcheck;
    Transform botcheck;

    public PlayerDragState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory)
    {}
    public override void EnterState()
    {
        InitializeSubstate();
        //Debug.Log("DRAGGING");
        Ctx.Ray.transform.SetParent(Ctx.Rigidbod.transform);
        Ctx.CanFlip = false;
        groundcheck = Ctx.GroundCheck;
        frontcheck = Ctx.FrontCheck;
        topcheck = Ctx.TopCheck;
        botcheck = Ctx.BotCheck;
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
        topcheck.SetParent(Ctx.Rigidbod.transform);
        botcheck.SetParent(Ctx.Rigidbod.transform);
        Ctx.CanFlip = true;
    }
    public override void CheckSwitchStates()
    {
        if(!Ctx.DragToggle)
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