using System.Collections.Generic;

namespace PSGJ15_DCSA.Core
{
    public class SequenceNode : Node
    {
        public SequenceNode() : base() { }

        public SequenceNode(List<Node> children) : base()
        {
            foreach (var child in children)
            {
                Attach(child);
            }
        }

        private void Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }

            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
}