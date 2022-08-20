using System.Collections.Generic;
enum PlayerStates {
    idle,
    run,
    grounded,
    crouch,
    drag,
    jump,
    attack,
}

public class PlayerStateFactory
{
    PlayerStateMachine _context;
    Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();
    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
        _states[PlayerStates.idle] = new PlayerIdleState(_context, this);
        _states[PlayerStates.run] = new PlayerRunningState(_context, this);
        _states[PlayerStates.grounded] = new PlayerGroundedState(_context, this);
        _states[PlayerStates.crouch] = new PlayerCrouchState(_context, this);
        _states[PlayerStates.drag] = new PlayerDragState(_context, this);
        _states[PlayerStates.jump] = new PlayerJumpingState(_context, this);
        _states[PlayerStates.attack] = new PlayerAttackingState(_context, this);
    }

    public PlayerBaseState Idle()
    {
        return _states[PlayerStates.idle];
    }
    public PlayerBaseState Run()
    {
        return _states[PlayerStates.run];
    }
    public PlayerBaseState Jump()
    {
        return _states[PlayerStates.jump];
    }
    public PlayerBaseState Attack()
    {
        return _states[PlayerStates.attack];
    }
    public PlayerBaseState Grounded()
    {
        return _states[PlayerStates.grounded];
    }
    public PlayerBaseState Crouch()
    {
        return _states[PlayerStates.crouch];
    }
    public PlayerBaseState Drag()
    {
        return _states[PlayerStates.drag];
    }
}
