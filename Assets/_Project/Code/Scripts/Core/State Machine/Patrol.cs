using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSGJ15_DCSA.Core
{
    public class Patrol : BaseState
    {
        private EnemyStateMachine m_StateMachine;

        public Patrol(EnemyStateMachine ennemyStateMachine) : base(nameof(Patrol), ennemyStateMachine)
        {
            m_StateMachine = ennemyStateMachine;
        }
    }
}
