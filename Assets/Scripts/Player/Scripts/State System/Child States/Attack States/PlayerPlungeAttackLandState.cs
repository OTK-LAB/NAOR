using UltimateCC;

public class PlayerPlungeAttackLandState : AttackState
{
    public PlayerPlungeAttackLandState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerData.Attack.AttackColliders.Find(x => x.Type == PlayerData.AttackStateVariables.AttackType.Plunge).Collider.enabled = true;
    }

    public override void Exit()
    {
        base.Exit();
        playerData.Attack.AttackColliders.Find(x => x.Type == PlayerData.AttackStateVariables.AttackType.Plunge).Collider.enabled = false;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void PhysicsCheck()
    {
        base.PhysicsCheck();
        if(localTime > playerData.Attack.PlungeAttack.LandDuration)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void SwitchStateLogic()
    {
        base.SwitchStateLogic();
    }

    public override void Update()
    {
        base.Update();
    }
}
