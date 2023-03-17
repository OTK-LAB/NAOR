using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Old_PlayerBusyState : Old_PlayerBaseState
{
    public Old_PlayerBusyState(PlayerController currentContext, Old_PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {
        IsRootState = true;
    }
    public override void EnterState()
    {
        Ctx.AppliedMovement = 0;   
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
        if(Ctx.CurrentCombatState == Ctx.CombatFactory.Peaceful())
        {
            if(Ctx.IsOnGround)
            {
                SwitchState(Factory.Grounded());
            }
            if(!Ctx.IsOnGround)
            {
                SwitchState(Factory.InAir());
            }
        }
    }

    public override void InitializeSubstate()
    {

    }
}
