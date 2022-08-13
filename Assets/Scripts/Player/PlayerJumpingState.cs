using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
    public PlayerJumpingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {
        IsRootState = true;
    }
    public override void EnterState()
    {
        Debug.Log("JUMP STATE");
        Ctx.Rigidbod.velocity = new Vector2(Ctx.Rigidbod.velocity.x, Ctx.JumpForce);
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

    }
}