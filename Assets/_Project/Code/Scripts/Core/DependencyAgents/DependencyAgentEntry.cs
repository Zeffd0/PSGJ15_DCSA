using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSGJ15_DCSA.Enums;

namespace PSGJ15_DCSA.Core.DependencyAgents
{
    [Serializable]
    public class DependencyAgentEntry
    {
        public DependencyAgentType type;
        public ScriptableObject agent;
    }
}
