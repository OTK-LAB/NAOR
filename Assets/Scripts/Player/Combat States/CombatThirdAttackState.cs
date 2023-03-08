using UnityEngine;

public class CombatThirdAttackState : CombatBaseState
{
    float endtime;
    public CombatThirdAttackState(PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory, float damage) :
    base(currentContext, combatStateFactory, movementStateFactory, damage)
    { }
    public override void EnterState()
    {
        Ctx.IsAttackPressed = false;
        Ctx.PlayerAnimator.Play("PlayerThirdAttack");
        endtime = Time.time + Ctx.PlayerAnimator.GetCurrentAnimatorStateInfo(0).length/2;
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
        if(Time.time >= endtime)
        {
            Ctx.LastAttack = 0;
            SwitchState(CombatFactory.Peaceful());
        }
    }
}
