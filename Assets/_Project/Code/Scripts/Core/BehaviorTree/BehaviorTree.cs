using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSGJ15_DCSA.Core.BehaviorTree
{
    public class BehaviorTree
    {
        private Node root = null;

        protected void Start()
        {
            root = SetupTree();
        }

        public virtual void Tick()
        {
            if (root != null)
                root.Evaluate();
        }

        protected virtual Node SetupTree()
        {
            return null;
        }
    }
}