using UnityEngine;

public class CombatPeacefulState : CombatBaseState
{
    public CombatPeacefulState(PlayerController currentContext, CombatStateFactory combatStateFactory, Old_PlayerStateFactory movementStateFactory, float damage):
    base(currentContext, combatStateFactory, movementStateFactory, damage){}

    private float attackBuffer;

    public override void EnterState()
    {
        attackBuffer = Time.time + 1;
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
        // F�X:: When plunge attack performed basic attack too performes
        if (!Ctx.CanNotPlunge && Ctx.IsAttackPressed && Ctx.IsDownPressed)
        {
            SwitchState(CombatFactory.PlungeAttack());
        }
       
        if (Ctx.IsAttackPressed && Ctx.CurrentMovementState.Query(MovementFactory.Standing()))
        {
            if(Time.time <= attackBuffer){
                switch(Ctx.LastAttack){
                    case 0:
                        SwitchState(CombatFactory.BasicAttack());
                        break;
                    case 1:
                        SwitchState(CombatFactory.SecondAttack());
                        break;
                    case 2:
                        SwitchState(CombatFactory.ThirdAttack());
                        break;
                }
            }
            else
            {
                Ctx.LastAttack = 0;
                SwitchState(CombatFactory.BasicAttack());
            }
        }
       
        if (Ctx.IsHeavyAttackPressed && Ctx.CurrentMovementState.Query(MovementFactory.Standing()))
        {
            SwitchState(CombatFactory.Charge());
        }
        
    }
}
