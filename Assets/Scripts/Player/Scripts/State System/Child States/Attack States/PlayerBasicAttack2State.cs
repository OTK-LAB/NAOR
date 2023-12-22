using UltimateCC;

public class PlayerBasicAttack2State : AttackState
{
    public PlayerBasicAttack2State(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
    {
    }

    public override void Enter()
    {
        base.Enter();
        attackDuration = playerData.Attack.BasicAttack2.AttackDuration;
        maxStateTime = playerData.Attack.BasicAttack2.MaxStateTime;
        inputManager.Input_Attack = false;
        playerData.Attack.AttackColliders.Find(x => x.Type == PlayerData.AttackStateVariables.AttackType.Basic2).Collider.enabled = true;
    }

    public override void Exit()
    {
        base.Exit();
        playerData.Attack.AttackColliders.Find(x => x.Type == PlayerData.AttackStateVariables.AttackType.Basic2).Collider.enabled = false;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    public override void PhysicsCheck()
    {
    }

    public override void SwitchStateLogic()
    {
        base.SwitchStateLogic();
        if (localTime > attackDuration)
        {
            if (localTime < maxStateTime && inputManager.Input_Attack)
            {
                stateMachine.ChangeState(player.BasicAttack3State);
            }
            else if (localTime > maxStateTime)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else if (inputManager.Input_Walk != 0)
            {
                stateMachine.ChangeState(player.WalkState);
            }
            else if (inputManager.Input_Jump)
            {
                stateMachine.ChangeState(player.JumpState);
            }
            else if (inputManager.Input_Dash)
            {
                stateMachine.ChangeState(player.DashState);
            }
            else if (inputManager.Input_Crouch)
            {
                stateMachine.ChangeState(player.CrouchIdleState);
            }
        }
    }

    public override void Update()
    {
        base.Update();
    }

}
