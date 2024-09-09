using System;
using UnityEngine;

namespace PSGJ15_DCSA.Core
{
    public class StateMachine
    {
        private BaseState _currentState;

        public StateMachine()
        {

        }

        public void UpdateStateMachine()
        {
            _currentState?.UpdateLogic();
        }

        public void ChangeState(BaseState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

        public BaseState GetCurrentState()
        {
            return _currentState;
        }

        protected virtual void SetInitialState(BaseState initialState)
        {
            _currentState = initialState;
            _currentState?.Enter();
        }
    }
}