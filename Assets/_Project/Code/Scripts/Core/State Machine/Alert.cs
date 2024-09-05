using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSGJ15_DCSA.Core
{
    public class Alert : BaseState
    {
        private EnemyStateMachine m_StateMachine;

        public Alert(EnemyStateMachine ennemyStateMachine) : base(nameof(Alert), ennemyStateMachine)
        {
            m_StateMachine = ennemyStateMachine;
        }
    }
}
