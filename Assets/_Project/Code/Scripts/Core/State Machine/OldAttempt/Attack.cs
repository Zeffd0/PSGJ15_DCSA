using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSGJ15_DCSA.Core
{
    public class Attack : BaseState
    {
        private EnemyStateMachine m_StateMachine;
        public Attack(EnemyStateMachine ennemyStateMachine) : base(nameof(Attack), ennemyStateMachine)
        {
            m_StateMachine = ennemyStateMachine;
        }

    }
}
