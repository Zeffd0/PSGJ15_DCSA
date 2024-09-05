namespace PSGJ15_DCSA.Core
{
    public class Retreat : BaseState
    {
        private EnemyStateMachine m_StateMachine;

        public Retreat(EnemyStateMachine ennemyStateMachine) : base(nameof(Retreat), ennemyStateMachine)
        {
            m_StateMachine = ennemyStateMachine;
        }
    }
}
