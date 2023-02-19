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

public class PlayerStateFactory
{
    PlayerController _context;
    Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();
    public PlayerStateFactory(PlayerController currentContext)
    {
        _context = currentContext;
        _states[PlayerStates.idle] = new PlayerIdleState(_context, this);
        _states[PlayerStates.run] = new PlayerRunningState(_context, this);
        _states[PlayerStates.grounded] = new PlayerGroundedState(_context, this);
        _states[PlayerStates.crouch] = new PlayerCrouchState(_context, this);
        _states[PlayerStates.jump] = new PlayerJumpingState(_context, this);
        _states[PlayerStates.dash] = new PlayerDashState(_context, this);
        _states[PlayerStates.inAir] = new PlayerInAirState(_context, this);
        _states[PlayerStates.fall] = new PlayerFallState(_context, this);
        _states[PlayerStates.standing] = new PlayerStandingState(_context, this);
        _states[PlayerStates.busy] = new PlayerBusyState(_context,this);

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
    }public PlayerBaseState Dash()
    {
        return _states[PlayerStates.dash];
    }
    public PlayerBaseState Grounded()
    {
        return _states[PlayerStates.grounded];
    }
    public PlayerBaseState Crouch()
    {
        return _states[PlayerStates.crouch];
    }
    public PlayerBaseState InAir()
    {
        return _states[PlayerStates.inAir];
    }
    public PlayerBaseState Fall()
    {
        return _states[PlayerStates.fall];
    }
    public PlayerBaseState Standing()
    {
        return _states[PlayerStates.standing];
    }
    public PlayerBaseState Busy()
    {
        return _states[PlayerStates.busy];
    }
}
