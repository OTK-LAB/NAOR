using UnityEngine;

public class CombatBasicAttackState : CombatBaseState
{
    public CombatBasicAttackState(PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory):
    base(currentContext, combatStateFactory, movementStateFactory){}
    public override void EnterState()
    {

        Ctx.PlayerAnimator.Play("PlayerBasicAttack");
        //SwitchState(CombatFactory.Peaceful());
    }
    public override void UpdateState(){

    }
    public override void ExitState()
    {

    }
    public override void CheckSwitchStates()
    {

    }

    
}
