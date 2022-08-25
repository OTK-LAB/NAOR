using UnityEngine;

public class PlayerClimbState : PlayerBaseState
{
    float waitTime;
    bool positionChanged;
    public PlayerClimbState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {
        IsRootState = true;
    }
    public override void EnterState()
    {
        positionChanged = false;
        waitTime = Time.time + 2;
        Ctx.CanFlip = false;
        Ctx.PlayerAnimator.Play("PlayerClimb");

        
    }
    public override void UpdateState()
    {
        if(Time.time >= waitTime && !positionChanged)
        {
            positionChanged = true;
            if(Ctx.FacingRight)
            {
                Ctx.Rigidbod.transform.position = new Vector2(Ctx.Rigidbod.transform.position.x + 2, Ctx.Rigidbod.transform.position.y + 3);
            }
            else
            {
                Ctx.Rigidbod.transform.position = new Vector2(Ctx.Rigidbod.transform.position.x -2, Ctx.Rigidbod.transform.position.y + 3);
            }
        }
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        Ctx.CanFlip = true;
        Ctx.Rigidbod.gravityScale = Ctx.DefaultGravity;
    }
    public override void CheckSwitchStates()
    {
        if(Time.time >= waitTime)
        {
            SwitchState(Factory.Grounded());
        }
    }
    public override void InitializeSubstate()
    {

    }

    public override string StateName()
    {
        return "Idle";
    }

    
}
