
using UnityEngine;

namespace PSGJ15_DCSA.Core
{
    public class BaseState
    {
        private readonly string m_name;
        protected StateMachine stateMachine;
        public BaseState(string name, StateMachine stateMachine)
        {
            m_name = name;
            this.stateMachine = stateMachine;
        }
        public virtual void Enter() 
        {
            Debug.Log(m_name); // passing in a private string is just to see in debug what state we're in
        }
        public virtual void UpdateLogic() { }
        public virtual void Exit() { }
    }
}
