using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSGJ15_DCSA.Core
{
    public class Run : BaseState
    {
        private EnemyStateMachine m_StateMachine;

        public Run(EnemyStateMachine ennemyStateMachine) : base(nameof(Run), ennemyStateMachine)
        {
            m_StateMachine = ennemyStateMachine;
        }
    }
}
