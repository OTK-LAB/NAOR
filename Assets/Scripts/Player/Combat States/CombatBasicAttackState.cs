using UnityEngine;

public class CombatBasicAttackState : CombatBaseState
{
    float endtime;
    public CombatBasicAttackState(PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory):
    base(currentContext, combatStateFactory, movementStateFactory){}
    public override void EnterState()
    {
        endtime = Time.time + 1.0f;
        Ctx.PlayerAnimator.Play("PlayerBasicAttack");   
    }
    public override void UpdateState(){
        if(Time.time > endtime)
            SwitchState(CombatFactory.Peaceful());
    }
    public override void ExitState()
    {

    }
    public override void CheckSwitchStates()
    {

    }

    
}
