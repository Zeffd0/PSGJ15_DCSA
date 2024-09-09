using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSGJ15_DCSA.Core
{
    public class Death : BaseState
    {
        private EnemyStateMachine m_StateMachine;

        public Death(EnemyStateMachine ennemyStateMachine) : base(nameof(Death), ennemyStateMachine)
        {
            m_StateMachine = ennemyStateMachine;
        }
    }
}
