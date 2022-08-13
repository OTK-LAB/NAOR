public class PlayerStateFactory
{
    PlayerStateMachine _context;

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
    }

    public PlayerBaseState Idle()
    {
        return new PlayerIdleState(_context, this);
    }
    public PlayerBaseState Run()
    {
        return new PlayerRunningState(_context, this);
    }
    public PlayerBaseState Jump()
    {
        return new PlayerJumpingState(_context, this);
    }
    public PlayerBaseState Attack()
    {
        return new PlayerAttackingState(_context, this);
    }
    public PlayerBaseState Grounded()
    {
        return new PlayerGroundedState(_context, this);
    }
}
