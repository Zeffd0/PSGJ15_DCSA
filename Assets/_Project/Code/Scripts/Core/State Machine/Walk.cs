using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSGJ15_DCSA.Core
{
    public class Walk : BaseState
    {
        private EnemyStateMachine m_StateMachine;

        public Walk(EnemyStateMachine ennemyStateMachine) : base(nameof(Walk), ennemyStateMachine)
        {
            m_StateMachine = ennemyStateMachine;
        }

    }
}
