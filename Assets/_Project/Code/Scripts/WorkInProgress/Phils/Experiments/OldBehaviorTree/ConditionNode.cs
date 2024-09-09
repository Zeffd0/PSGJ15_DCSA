namespace PSGJ15_DCSA.Core
{
    public abstract class ConditionNode : Node
    {
        public ConditionNode() : base() { }

        public override NodeState Evaluate()
        {
            bool result = CheckCondition();
            return result ? NodeState.SUCCESS : NodeState.FAILURE;
        }

        protected abstract bool CheckCondition();
    }
}