namespace PSGJ15_DCSA.Core
{
    public class EnemyStateMachine : StateMachine
    {
        public Idle idle {get;}
        public Walk walk {get;}
        public Attack attack {get;}
        public EnemyStateMachine()
        {
            idle = new Idle(this);
            walk = new Walk(this);
            attack = new Attack(this);

            // BaseState initialState = idle;
            // ChangeState(initialState);
            // Debug.Log("hehe" + initialState);
        }
    }
}
