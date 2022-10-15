using System.Collections.Generic;

enum CombatStates {
    peaceful,
    basicAttack
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

        _states[CombatStates.peaceful] = new CombatPeacefulState(_context, this, _movementFactory);
        _states[CombatStates.basicAttack] = new CombatBasicAttackState(_context, this, _movementFactory);
    }

    public CombatBaseState Peaceful()
    {
        return _states[CombatStates.peaceful];
    }
    public CombatBaseState BasicAttack()
    {
        return _states[CombatStates.basicAttack];
    }
}
