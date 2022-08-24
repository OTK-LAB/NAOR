using UnityEngine;

public abstract class PlayerBaseState
{
    private bool _isRootState = false;
    private PlayerStateMachine _ctx;
    private PlayerStateFactory _factory;
    private PlayerBaseState _currentSuperState;
    private PlayerBaseState _currentSubState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected PlayerStateMachine Ctx { get { return _ctx; } }
    protected PlayerStateFactory Factory { get { return _factory;} }
    protected PlayerBaseState SuperState { get {return _currentSuperState;}}
    protected PlayerBaseState SubState { get {return _currentSubState;}}
    
    public PlayerBaseState( PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
    }
    
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubstate();
    public abstract string StateName();

    public void UpdateStates()
    {
        UpdateState();
        if(_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }
    protected void SwitchState(PlayerBaseState newState)
    {
        //current state exits state
        ExitState();

        if(newState._isRootState){
            // switch current state of context
            _ctx.CurrentState = newState;
        }
        else if(_currentSuperState != null)
        {
            // switch hierarchical relations
            _currentSuperState.SetSubState(newState);
            
        }
        
        //new state enters state
        newState.EnterState();

        Ctx.CurrentState.PrintCurrentHierarchy();
    }
    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }
    protected void SetSubState(PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
    protected void PrintCurrentHierarchy()
    {
        Debug.Log(this);
        if(_currentSubState != null)
        {
            _currentSubState.PrintCurrentHierarchy();
        }
        else
        {
            Debug.Log("-------------------------");
        }
    }
}
