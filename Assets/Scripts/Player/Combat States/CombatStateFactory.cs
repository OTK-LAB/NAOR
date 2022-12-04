using System.Collections.Generic;

enum CombatStates {
    peaceful,
    basicAttack,
    heavyAttack,
    charge,
}
public class CombatStateFactory
{
    PlayerController _context;
    PlayerStateFactory _movementFactory;
    Dictionary<CombatStates, CombatBaseState> _states = new Dictionary<CombatStates, CombatBaseState>();
    public CombatStateFactory(PlayerController currentContext, PlayerStateFactory movementFactory)
    {
        _context = currentContext;
        _movementFactory = movementFactory;

        _states[CombatStates.peaceful] = new CombatPeacefulState(_context, this, _movementFactory, 0);
        _states[CombatStates.basicAttack] = new CombatBasicAttackState(_context, this, _movementFactory, 10);
        _states[CombatStates.heavyAttack] = new CombatHeavyAttackState(_context, this, _movementFactory, 10);
        _states[CombatStates.charge] = new CombatChargeState(_context, this, _movementFactory, 0);
    }

    public CombatBaseState Peaceful()
    {
        return _states[CombatStates.peaceful];
    }
    public CombatBaseState BasicAttack()
    {
        return _states[CombatStates.basicAttack];
    }
    public CombatBaseState HeavyAttack()
    {
        return _states[CombatStates.heavyAttack];
    } public CombatBaseState Charge()
    {
        return _states[CombatStates.charge];
    }
    
}
