using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
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
        //FIXME: bu if-else yapisinin degismesi gerekebilir
        if (Ctx.IsDashPressed && Ctx.CanDash)
        {
            SwitchState(Factory.Dash());
        }
        if(!Ctx.IsOnGround)
        {
            SwitchState(Factory.InAir());
        }
        if(Ctx.CurrentCombatState != Ctx.CombatFactory.Peaceful())
        {
            SwitchState(Factory.Busy());
        }
    }
    public override void InitializeSubstate()
    {
        SetSubState(Factory.Standing());   
    }
}