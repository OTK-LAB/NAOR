using UnityEngine;

public class CombatThirdAttackState : CombatBaseState
{
    float endtime;
    public CombatThirdAttackState(PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory, float damage) :
    base(currentContext, combatStateFactory, movementStateFactory, damage)
    { }
    public override void EnterState()
    {
        Ctx.ComboTriggered = false;
        Ctx.PlayerAnimator.Play("PlayerThirdAttack");
        endtime = Time.time + Ctx.PlayerAnimator.GetCurrentAnimatorStateInfo(0).length;
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
            SwitchState(CombatFactory.Peaceful());
        }
    }
}
