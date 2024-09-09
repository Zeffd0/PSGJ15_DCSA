namespace PSGJ15_DCSA.Core
{
    public abstract class ActionNode : Node
    {
        public ActionNode() : base() { }

        public override NodeState Evaluate()
        {
            if (DoAction())
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }

        protected abstract bool DoAction();
    }
}