using System.Collections;
using System.Collections.Generic;
using PSGJ15_DCSA.Enums;
using UnityEngine;

namespace PSGJ15_DCSA.Core.DependencyAgents
{
    [CreateAssetMenu(fileName = "REG_DependencyAgents", menuName = "Project/DependencyAgents/Registry", order = 0)]
    public class REG_DependencyAgents : ScriptableObject
    {
        private static REG_DependencyAgents _instance = null;
        
        public static REG_DependencyAgents Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = Resources.Load<REG_DependencyAgents>("ScriptableObjects/DependencyAgents/REG_DependencyAgents");
                    if(_instance == null)
                    {
                        Debug.LogError("'REG_DependencyAgents' instance not found.");
                    }
                }
                return _instance;
            }
        }

        [SerializeField]
        private List<DependencyAgentEntry> DependencyAgentsEntries = new List<DependencyAgentEntry>();
        
        public ScriptableObject GetDependencyAgent(DependencyAgentType type)
        {
            foreach(DependencyAgentEntry entry in DependencyAgentsEntries)
            {
                if(entry.type == type)
                {
                    return entry.agent;
                }
            }

            Debug.Log($"Dependency Agent not found for type : {type}.");
            return null;
        }
    }
}
