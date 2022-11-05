using UnityEngine;
using TMPro;

public abstract class PlayerBaseState
{
    private bool _isRootState = false;
    private PlayerController _ctx;
    private PlayerStateFactory _factory;
    private PlayerBaseState _currentSuperState;
    private PlayerBaseState _currentSubState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected PlayerController Ctx { get { return _ctx; } }
    protected PlayerStateFactory Factory { get { return _factory;} }
    public PlayerBaseState SuperState { get {return _currentSuperState;}}
    public PlayerBaseState SubState { get {return _currentSubState;}}
    
    public PlayerBaseState( PlayerController currentContext, PlayerStateFactory playerStateFactory)
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
    protected void SwitchState(PlayerBaseState newState)
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
    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }
    protected void SetSubState(PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
        _currentSubState.EnterState();
    }
    protected void PrintCurrentHierarchy()
    {
        Debug.Log(this);
        Ctx._movementHierarchyText.SetText(Ctx._movementHierarchyText.text + this + "\n");
        if(_currentSubState != null)
        {
            _currentSubState.PrintCurrentHierarchy();
        }
        else
        {
            Debug.Log("-------------------------");
        }
    }

    public bool Query(PlayerBaseState query)
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
