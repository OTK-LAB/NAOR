using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {}
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
        if(Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Run());
        }
    }
    public override void InitializeSubstate()
    {

    }
}
