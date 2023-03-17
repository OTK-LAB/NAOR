using UnityEngine;

public class CombatFourthAttackState : CombatBaseState
{
    float endtime;
    public CombatFourthAttackState(PlayerController currentContext, CombatStateFactory combatStateFactory, Old_PlayerStateFactory movementStateFactory, float damage) :
    base(currentContext, combatStateFactory, movementStateFactory, damage)
    { }
    public override void EnterState()
    {
        Ctx.ComboTriggered = false;
        Ctx.PlayerAnimator.Play("PlayerFourthAttackState");
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
            if(Ctx.ComboTriggered)
            {
                SwitchState(CombatFactory.FifthAttack());
            }
            else
            {
                SwitchState(CombatFactory.Peaceful());
            }
        }
    }
}
