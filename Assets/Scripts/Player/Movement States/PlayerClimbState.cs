using UnityEngine;

public class PlayerClimbState : PlayerBaseState
{
    float waitTime;
    bool positionChanged;
    public PlayerClimbState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
    :base (currentContext, playerStateFactory) {
    }
    public override void EnterState()
    {
        positionChanged = false;
        waitTime = Time.time + 1.5f;
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
                Ctx.Rigidbod.transform.position = new Vector2(Ctx.Rigidbod.transform.position.x + 2, Ctx.Rigidbod.transform.position.y + 2);
            }
            else
            {
                Ctx.Rigidbod.transform.position = new Vector2(Ctx.Rigidbod.transform.position.x -2, Ctx.Rigidbod.transform.position.y + 2);
            }
            SwitchState(Factory.Standing());

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

    }
    public override void InitializeSubstate()
    {

    }

}
