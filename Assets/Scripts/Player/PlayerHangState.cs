using UnityEngine;

public class PlayerHangState : PlayerBaseState
{
    public PlayerHangState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {
        IsRootState = true;
    }
    public override void EnterState()
    {
        Ctx.Rigidbod.gravityScale = 0;
        Ctx.Rigidbod.velocity = new Vector2(0.0f, 0.0f);
        Ctx.PlayerAnimator.Play("PlayerHang");
        Ctx.CanFlip = false;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        Ctx.CanFlip = true;   
    }
    public override void CheckSwitchStates()
    {
        if(Ctx.IsJumpPressed)
        {
            SwitchState(Factory.Climb());
        }
    }
    public override void InitializeSubstate()
    {
        
    }

    public override string StateName()
    {
        return "Hang";
    }
}
