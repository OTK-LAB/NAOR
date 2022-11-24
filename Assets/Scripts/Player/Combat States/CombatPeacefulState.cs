using UnityEngine;

public class CombatPeacefulState : CombatBaseState
{
    public CombatPeacefulState(PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory, float damage):
    base(currentContext, combatStateFactory, movementStateFactory, damage){}
    public override void EnterState()
    {

    }
    public override void UpdateState(){
        CheckSwitchStates();
    }
    public override void ExitState()
    {
   
    }
    public override void CheckSwitchStates()
    {
        if(Ctx.IsAttackPressed && Ctx.CurrentMovementState.Query(MovementFactory.Standing()))
        {
            SwitchState(CombatFactory.BasicAttack());
            Debug.Log("ben temizlemedim hala");
        }
      //  if (Ctx.IsHeavyAttackPressed && Ctx.CurrentMovementState.Query(MovementFactory.Standing()))
      //  {
      //      SwitchState(CombatFactory.HeavyAttack());
      //      Debug.Log("ben temizlemedim hala");
      //  }
    }
}
