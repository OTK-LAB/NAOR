using UnityEngine;
using TMPro;

public abstract class Old_PlayerBaseState
{
    private bool _isRootState = false;
    private PlayerController _ctx;
    private Old_PlayerStateFactory _factory;
    private Old_PlayerBaseState _currentSuperState;
    private Old_PlayerBaseState _currentSubState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected PlayerController Ctx { get { return _ctx; } }
    protected Old_PlayerStateFactory Factory { get { return _factory;} }
    public Old_PlayerBaseState SuperState { get {return _currentSuperState;}}
    public Old_PlayerBaseState SubState { get {return _currentSubState;}}
    
    public Old_PlayerBaseState( PlayerController currentContext, Old_PlayerStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
    }
    
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubstate();
    public void UpdateStates()
    {
        UpdateState();
        if(_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }
    protected void SwitchState(Old_PlayerBaseState newState)
    {
        //current state exits state
        ExitState();

        if(newState._isRootState){
            // switch current state of context
            _ctx.CurrentMovementState = newState;
            _currentSubState = null;

        }
        else if(_currentSuperState != null)
        {
            // switch hierarchical relations
            _currentSuperState.SetSubState(newState);
            
        }
        
        //new state enters state
        newState.EnterState();
        Ctx._movementHierarchyText.SetText(string.Empty);
        Ctx.CurrentMovementState.PrintCurrentHierarchy();
    }
    protected void SetSuperState(Old_PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }
    protected void SetSubState(Old_PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
        _currentSubState.EnterState();
    }
    protected void PrintCurrentHierarchy()
    {
        Ctx._movementHierarchyText.SetText(Ctx._movementHierarchyText.text + this + "\n");
        if(_currentSubState != null)
        {
            _currentSubState.PrintCurrentHierarchy();
        }
    }

    public bool Query(Old_PlayerBaseState query)
    {
        if(this == query)
        {
            return true;
        }
        if(_currentSubState != null)
        {
            return _currentSubState.Query(query);
        }
        return false;
    }
}
