using UnityEngine;

public class CombatPeacefulState : CombatBaseState
{
    public CombatPeacefulState(PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory, float damage):
    base(currentContext, combatStateFactory, movementStateFactory, damage){}
    public override void EnterState()
    {
        Ctx.CanMove = true;
        Ctx.CanFlip = true;
        Ctx.ChargeCanceled = false;
    }
    public override void UpdateState(){
        CheckSwitchStates();
    }
    public override void ExitState()
    {
   
    }
    public override void CheckSwitchStates()
    {
        // FÝX:: When plunge attack performed basic attack too performes
        if (!Ctx._canNotPlunge && Ctx.IsAttackPressed && Ctx._isDownPressed)
        {
            SwitchState(CombatFactory.PlungeAttack());
        }
        if (Ctx.IsAttackPressed && Ctx.CurrentMovementState.Query(MovementFactory.Standing()))
        {
            SwitchState(CombatFactory.BasicAttack());
        }
        if (Ctx.IsHeavyAttackPressed && Ctx.CurrentMovementState.Query(MovementFactory.Standing()))
        {
            SwitchState(CombatFactory.Charge());
        }
        
    }
}
