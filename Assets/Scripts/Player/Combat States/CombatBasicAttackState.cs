using UnityEngine;

public class CombatBasicAttackState : CombatBaseState
{
    float endtime;
    public CombatBasicAttackState(PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory, float damage):
    base(currentContext, combatStateFactory, movementStateFactory, damage){}
    public override void EnterState()
    {
        Ctx.CanMove = false;
        Ctx.PlayerAnimator.Play("PlayerBasicAttack");
        endtime = Time.time + Ctx.PlayerAnimator.GetCurrentAnimatorStateInfo(0).length;
    }
    public override void UpdateState(){
        if(Time.time >= endtime)
        {
            SwitchState(CombatFactory.Peaceful());
        }
    }
    public override void ExitState()
    {
        Ctx.CanMove = true;
        //FIXME:
        //  Moves player forward after attacking
        Ctx.transform.position = new Vector2( Ctx.transform.position.x + (Ctx.FacingRight ? .57f : -0.57f), Ctx.transform.position.y);
    }
    public override void CheckSwitchStates()
    {
    }
    public void OnBasicAttackEnded()
    {
        SwitchState(CombatFactory.Peaceful());
    }
    
}
