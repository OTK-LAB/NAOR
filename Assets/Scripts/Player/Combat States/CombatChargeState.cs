using UnityEngine;

public class CombatChargeState : CombatBaseState
{
    public CombatChargeState(PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory, float damage) :
     base(currentContext, combatStateFactory, movementStateFactory, damage)
    { }

    public override void EnterState()
    {
        Ctx.PlayerAnimator.Play("PlayerCharge");
        Ctx.IsHeavyAttackPressed = false;
        Ctx.CanMove = false;
        Ctx.CanFlip = false;
        
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
        if (Ctx.CanHeavyAttack)
        {
            SwitchState(CombatFactory.HeavyAttack());
        }
        if (Ctx.ChargeCanceled)
        {
            SwitchState(CombatFactory.Peaceful());
        }
    }
}
