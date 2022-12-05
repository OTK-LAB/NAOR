using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBusyState : PlayerBaseState
{
    public PlayerBusyState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {
        IsRootState = true;
    }
    public override void EnterState()
    {
        //FIX WITH canMove MAYBE
        Debug.Log("BUSY GİRDİ");
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
