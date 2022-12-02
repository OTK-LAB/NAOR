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
        Debug.Log("Enter Heavy");
        Ctx.PlayerAnimator.Play("PlayerHeavyAttack");
        endtime = Time.time + Ctx.PlayerAnimator.GetCurrentAnimatorStateInfo(0).length;
    }
    public override void UpdateState()
    {
       if (Time.time >= endtime)
       {
           SwitchState(CombatFactory.Peaceful());
       }
    }
    public override void ExitState()
    {
            
    }
    public override void CheckSwitchStates()
    {

    }

}
