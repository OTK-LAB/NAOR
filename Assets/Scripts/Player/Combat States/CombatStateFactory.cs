using System.Collections.Generic;

enum CombatStates {
    peaceful,
    basicAttack,
    secondAttack,
    thirdAttack,
    fourthAttack,
    fifthAttack,
    heavyAttack,
    charge,
    plungeAttack,
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
        _states[CombatStates.secondAttack] = new CombatSecondAttackState(_context,this, _movementFactory, 15);
        _states[CombatStates.thirdAttack] = new CombatThirdAttackState(_context, this, _movementFactory, 20);
        _states[CombatStates.fourthAttack] = new CombatFourthAttackState(_context, this, _movementFactory, 25);
        _states[CombatStates.fifthAttack] = new CombatFifthAttackState(_context, this, _movementFactory, 30);
        _states[CombatStates.heavyAttack] = new CombatHeavyAttackState(_context, this, _movementFactory, 10);
        _states[CombatStates.charge] = new CombatChargeState(_context, this, _movementFactory, 0);
        _states[CombatStates.plungeAttack] = new CombatPlungeAttackState(_context, this, _movementFactory, 75);
    }

    public CombatBaseState Peaceful()
    {
        return _states[CombatStates.peaceful];
    }
    public CombatBaseState BasicAttack()
    {
        return _states[CombatStates.basicAttack];
    }
    public CombatBaseState SecondAttack()
    {
        return _states[CombatStates.secondAttack];
    }
    public CombatBaseState ThirdAttack()
    {
        return _states[CombatStates.thirdAttack];
    }
    public CombatBaseState FourthAttack()
    {
        return _states[CombatStates.fourthAttack];
    }
    public CombatBaseState FifthAttack()
    {
        return _states[CombatStates.fifthAttack];
    }
    public CombatBaseState HeavyAttack()
    {
        return _states[CombatStates.heavyAttack];
    } 
    public CombatBaseState Charge()
    {
        return _states[CombatStates.charge];
    }
    public CombatBaseState PlungeAttack()
    {
        return _states[CombatStates.plungeAttack];
    }
    
}
