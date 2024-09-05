using System;
using System.Collections.Generic;

namespace PSGJ15_DCSA.Core.SimplifiedBehaviorTree
{
    public enum NodeState { RUNNING, SUCCESS, FAILURE }

    public abstract class Node
    {
        public abstract NodeState Evaluate();
    }

    public class Sequence : Node
    {
        private List<Node> children = new List<Node>();

        public override NodeState Evaluate()
        {
            foreach (var child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.FAILURE:
                        return NodeState.FAILURE;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        return NodeState.RUNNING;
                    default:
                        continue;
                }
            }
            return NodeState.SUCCESS;
        }

        public void AddChild(Node child) => children.Add(child);
    }

    public class Selector : Node
    {
        private List<Node> children = new List<Node>();

        public override NodeState Evaluate()
        {
            foreach (var child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        return NodeState.SUCCESS;
                    case NodeState.RUNNING:
                        return NodeState.RUNNING;
                    default:
                        continue;
                }
            }
            return NodeState.FAILURE;
        }

        public void AddChild(Node child) => children.Add(child);
    }

    public class ActionNode : Node
    {
        private Func<NodeState> action;

        public ActionNode(Func<NodeState> action)
        {
            this.action = action;
        }

        public override NodeState Evaluate() => action();
    }

    public class ConditionNode : Node
    {
        private Func<bool> condition;

        public ConditionNode(Func<bool> condition)
        {
            this.condition = condition;
        }

        public override NodeState Evaluate()
        {
            return condition() ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
