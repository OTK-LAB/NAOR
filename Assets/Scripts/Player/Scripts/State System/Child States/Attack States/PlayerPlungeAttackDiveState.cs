using UnityEngine;
using UltimateCC;

public class PlayerPlungeAttackDiveState : AttackState, IMove1D
{
    public PlayerPlungeAttackDiveState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
    {
    }
    RaycastHit2D breakableCheck;
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
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
        if(playerData.Physics.IsGrounded)
        {
            stateMachine.ChangeState(player.PlungeAttackLandState);
        }
    }

    public override void Update()
    {
        base.Update();
        breakableCheck = Physics2D.Raycast(player.Rigidbody2D.position, new Vector2(0, -1), 2f,playerData.Check.GroundLayer);
        if (breakableCheck.collider != null)
        {
            if (breakableCheck.collider.CompareTag("Breakable"))
            {
                breakableCheck.collider.gameObject.SetActive(false);
            }
        }
    }

    public void Move1D()
    {
        float _velocity = 0f;
        float _curveTime = localTime - playerData.Attack.PlungeAttack.WaitDuration;
        if (localTime > playerData.Attack.PlungeAttack.WaitDuration)
        {
            if (localTime > playerData.Attack.PlungeAttack.SpeedUpTime)
            {
                _velocity = playerData.Attack.PlungeAttack.SpeedUpCurve.Evaluate(_curveTime / playerData.Attack.PlungeAttack.SpeedUpTime);
            }
            else
            {
                _velocity = playerData.Attack.PlungeAttack.SpeedUpCurve.Evaluate(1f);
            }
        }
        _velocity *= playerData.Attack.PlungeAttack.MinYVelocity;
        rigidbody2D.velocity = new Vector2(0, _velocity);
    }
}
