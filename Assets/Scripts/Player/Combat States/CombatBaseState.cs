using UnityEngine;

public abstract class CombatBaseState
{
    private PlayerController _ctx;
    private CombatStateFactory _combatFactory;
    private PlayerStateFactory _movementFactory;
    private float _damageAmount = 0;

    public float DamageAmount { get {return _damageAmount;}} 
    protected PlayerController Ctx { get { return _ctx;}}
    protected CombatStateFactory CombatFactory { get { return _combatFactory; }}
    protected PlayerStateFactory MovementFactory{ get { return _movementFactory;}}

    public CombatBaseState( PlayerController currentContext, CombatStateFactory combatStateFactory, PlayerStateFactory movementStateFactory, float damage)
    {
        _ctx = currentContext; 
        _combatFactory = combatStateFactory;
        _movementFactory = movementStateFactory;
        _damageAmount = damage;
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
            Ctx._combatStateText.SetText(Ctx.CurrentCombatState.ToString());
        }
    }

    public bool CheckMovementState(PlayerBaseState query)
    {
        return Ctx.CurrentMovementState.Query(query);
    }
}
