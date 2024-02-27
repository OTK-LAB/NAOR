using System;

namespace UltimateCC
{
    [System.Serializable]
    public class PlayerStateMachine
    {
        public event Action<PlayerMain.AnimName> OnStateEnter;
        public event Action<PlayerMain.AnimName> OnStateExit;

        // Declaration of current runtime state
        public MainState CurrentState { get; private set; }

        // Function to initialize starting state
        public void Initialize(MainState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }

        // Function to change current state
        public void ChangeState(MainState newState)
        {
            CurrentState.Exit();
            OnStateExit?.Invoke(CurrentState._animEnum);
            CurrentState = newState;
            CurrentState.Enter();
            OnStateEnter?.Invoke(CurrentState._animEnum);

        }
    }
}
