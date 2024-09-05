using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSGJ15_DCSA.Core
{
    public class Stunned : BaseState
    {
        private EnemyStateMachine m_StateMachine;

        public Stunned(EnemyStateMachine ennemyStateMachine) : base(nameof(Stunned), ennemyStateMachine)
        {
            m_StateMachine = ennemyStateMachine;
        }
    }
}