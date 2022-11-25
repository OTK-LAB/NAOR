using UnityEngine;

public class CombatHeavyAttackState : CombatBaseState
{
    float endtime;

    public CombatHeavyAttackState(PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory, float damage) :
    base(currentContext, combatStateFactory, movementStateFactory, damage)
    { }
    public override void EnterState()
    {
        Ctx.IsHeavyAttackPressed = false;
        Debug.Log("Enter Heavy");
        Ctx.CanMove = false;
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
        Ctx.CanMove = true;
    }
    public override void CheckSwitchStates()
    {

    }

}
