using UnityEngine;

public class CombatHeavyAttackState : CombatBaseState
{
    float endtime;

    public CombatHeavyAttackState(PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory, float damage) :
    base(currentContext, combatStateFactory, movementStateFactory, damage)
    { }
    public override void EnterState()
    {
        Ctx.CanHeavyAttack = false;
        Ctx.PlayerAnimator.Play("PlayerHeavyAttack");
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
        if (Time.time >= endtime)
        {
            SwitchState(CombatFactory.Peaceful());
        }

    }

}
