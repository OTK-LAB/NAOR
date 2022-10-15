using UnityEngine;

public class PlayerInAirState : PlayerBaseState
{
    public PlayerInAirState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    public override void EnterState()
    {
        InitializeSubstate();
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
        if(Ctx.IsOnGround)
        {
            SwitchState(Factory.Grounded());
        }
    }
    public override void InitializeSubstate()
    {
        if(Ctx.Rigidbod.velocity.y > 0)
        {
            SetSubState(Factory.Jump());
        }
        if(Ctx.Rigidbod.velocity.y < 0)
        {
            SetSubState(Factory.Fall());
        }
    }
}