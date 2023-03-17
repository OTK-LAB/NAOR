using System.Collections.Generic;
enum PlayerStates {
    idle,
    run,
    grounded,
    crouch,
    jump,
    dash,
    inAir,
    fall,
    standing,
    busy,
}

public class Old_PlayerStateFactory
{
    PlayerController _context;
    Dictionary<PlayerStates, Old_PlayerBaseState> _states = new Dictionary<PlayerStates, Old_PlayerBaseState>();
    public Old_PlayerStateFactory(PlayerController currentContext)
    {
        _context = currentContext;
        _states[PlayerStates.idle] = new Old_PlayerIdleState(_context, this);
        _states[PlayerStates.run] = new Old_PlayerRunningState(_context, this);
        _states[PlayerStates.grounded] = new Old_PlayerGroundedState(_context, this);
        _states[PlayerStates.crouch] = new Old_PlayerCrouchState(_context, this);
        _states[PlayerStates.jump] = new Old_PlayerJumpingState(_context, this);
        _states[PlayerStates.dash] = new Old_PlayerDashState(_context, this);
        _states[PlayerStates.inAir] = new Old_PlayerInAirState(_context, this);
        _states[PlayerStates.fall] = new Old_PlayerFallState(_context, this);
        _states[PlayerStates.standing] = new Old_PlayerStandingState(_context, this);
        _states[PlayerStates.busy] = new Old_PlayerBusyState(_context,this);

    }

    public Old_PlayerBaseState Idle()
    {
        return _states[PlayerStates.idle];
    }
    public Old_PlayerBaseState Run()
    {
        return _states[PlayerStates.run];
    }
    public Old_PlayerBaseState Jump()
    {
        return _states[PlayerStates.jump];
    }public Old_PlayerBaseState Dash()
    {
        return _states[PlayerStates.dash];
    }
    public Old_PlayerBaseState Grounded()
    {
        return _states[PlayerStates.grounded];
    }
    public Old_PlayerBaseState Crouch()
    {
        return _states[PlayerStates.crouch];
    }
    public Old_PlayerBaseState InAir()
    {
        return _states[PlayerStates.inAir];
    }
    public Old_PlayerBaseState Fall()
    {
        return _states[PlayerStates.fall];
    }
    public Old_PlayerBaseState Standing()
    {
        return _states[PlayerStates.standing];
    }
    public Old_PlayerBaseState Busy()
    {
        return _states[PlayerStates.busy];
    }
}
