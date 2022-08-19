using UnityEngine;

public class PlayerRunningState : PlayerBaseState
{
    public PlayerRunningState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {}
    public override void EnterState()
    {
        Debug.Log("RUNNING");
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
        Ctx.AppliedMovement = Ctx.CurrentMovementInput.x * Ctx.MovementSpeed;
    }
    public override void ExitState()
    {

    }
    public override void CheckSwitchStates()
    {
        if(!Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }
    }
    public override void InitializeSubstate()
    {

    }
}
