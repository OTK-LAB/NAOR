using UnityEngine;

public class CombatHeavyAttackState : CombatBaseState
{
    public CombatHeavyAttackState(PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory, float damage) :
    base(currentContext, combatStateFactory, movementStateFactory, damage)
    { }
    public override void EnterState()
    {
        Debug.Log("Enter Heavy");
    }
    public override void UpdateState()
    {
        Debug.Log("Update Heavy");

    }
    public override void ExitState()
    {
        Debug.Log("Exit Heavy");

    }
    public override void CheckSwitchStates()
    {

    }

}
