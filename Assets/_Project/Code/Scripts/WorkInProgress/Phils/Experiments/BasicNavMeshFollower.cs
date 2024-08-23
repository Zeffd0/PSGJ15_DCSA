using UnityEngine;
using UnityEngine.AI;

public class BasicNavMeshFollower : MonoBehaviour
{
    public Transform target;  // Assign this in the Inspector
    private NavMeshAgent agent;

    void Start()
    {
        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();
        
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found!");
        }
    }

    void Update()
    {
        if (target != null && agent != null)
        {
            // Set the destination to the target's position
            agent.SetDestination(target.position);
        }
    }
}