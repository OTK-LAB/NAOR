using UnityEngine;
using UltimateCC;

public class PlayerSwingState : MainState, IMove1D
{
    public PlayerSwingState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.transform.position = playerData.Swing.SwingInitialPosition;
        Debug.Log(rigidbody2D.linearVelocity);
        playerData.Physics.ConnectedHingeJoint.connectedBody = rigidbody2D;
    }

    public override void Exit()
    {
        base.Exit();
        playerData.Physics.ConnectedHingeJoint.connectedBody = null;
        playerData.Physics.ConnectedHingeJoint = null;
        rigidbody2D.linearDamping = 0f;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Move1D();
    }

    public override void PhysicsCheck()
    {
        base.PhysicsCheck();
    }

    public override void SwitchStateLogic()
    {
        base.SwitchStateLogic();
        if (inputManager.Input_Jump)
        {
            stateMachine.ChangeState(player.JumpState);
        }
    }

    public override void Update()
    {
        base.Update();
    }

    public void Move1D()
    {
        SwingMovement();
    }

    public void SwingMovement()
    {
        rigidbody2D.gravityScale = playerData.Swing.Gravity;
        rigidbody2D.linearDamping = playerData.Swing.Drag;
    }
}
