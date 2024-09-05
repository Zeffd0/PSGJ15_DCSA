using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSGJ15_DCSA.Core
{
    public class Idle : BaseState
    {
        private EnemyStateMachine m_StateMachine;

        public Idle(EnemyStateMachine ennemyStateMachine) : base(nameof(Idle), ennemyStateMachine)
        {
            m_StateMachine = ennemyStateMachine;
        }

        public override void Enter() 
        {
            
        }
    }
}
