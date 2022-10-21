using UnityEngine;

public class CombatBasicAttackState : CombatBaseState
{
    float endtime;
    public CombatBasicAttackState(PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory):
    base(currentContext, combatStateFactory, movementStateFactory){}
    public override void EnterState()
    {
        Ctx.PlayerAnimator.Play("PlayerBasicAttack");   
    }
    public override void UpdateState(){

    }
    public override void ExitState()
    {

    }
    public override void CheckSwitchStates()
    {

    }
    public void OnBasicAttackEnded()
    {
        SwitchState(CombatFactory.Peaceful());
    }
    
}
