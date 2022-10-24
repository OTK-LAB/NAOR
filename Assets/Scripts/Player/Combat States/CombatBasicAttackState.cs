using UnityEngine;

public class CombatBasicAttackState : CombatBaseState
{
    float endtime;
    public CombatBasicAttackState(PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory):
    base(currentContext, combatStateFactory, movementStateFactory){}
    public override void EnterState()
    {
        Ctx.PlayerAnimator.Play("PlayerBasicAttack");
        endtime = Time.time + 0.5f;
    }
    public override void UpdateState(){
        if(Time.time >= endtime)
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
    public void OnBasicAttackEnded()
    {
        SwitchState(CombatFactory.Peaceful());
    }
    
}
