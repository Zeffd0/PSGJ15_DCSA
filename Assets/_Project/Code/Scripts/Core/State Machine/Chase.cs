using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSGJ15_DCSA.Core
{
    public class Chase : BaseState
    {
        private EnemyStateMachine m_StateMachine;

        public Chase(EnemyStateMachine ennemyStateMachine) : base(nameof(Chase), ennemyStateMachine)
        {
            m_StateMachine = ennemyStateMachine;
        }
    }
}
