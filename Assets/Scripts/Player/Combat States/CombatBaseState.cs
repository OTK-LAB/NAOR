using UnityEngine;

public abstract class CombatBaseState
{
    private PlayerController _ctx;
    private CombatStateFactory _combatFactory;
    private PlayerStateFactory _movementFactory;

    protected PlayerController Ctx { get { return _ctx;}}
    protected CombatStateFactory CombatFactory { get { return _combatFactory; }}
    protected PlayerStateFactory MovementFactory{ get { return _movementFactory;}}

    public CombatBaseState( PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory)
    {
        _ctx = currentContext; 
        _combatFactory = combatStateFactory;
        _movementFactory = movementStateFactory;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    
    protected void SwitchState(CombatBaseState newState) {
        {
            ExitState();
            Ctx.CurrentCombatState = newState;
            newState.EnterState();
        }
    }

    public bool CheckMovementState(PlayerBaseState query)
    {
        return Ctx.CurrentMovementState.Query(query);
    }
}
