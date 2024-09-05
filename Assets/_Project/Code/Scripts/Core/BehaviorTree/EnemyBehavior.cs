using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace PSGJ15_DCSA.Core.SimplifiedBehaviorTree
{
    public class EnemyBehavior
    {
        private Node m_root;
        private Enemy m_enemyReference;
        private Animator m_animator;
        private NavMeshAgent m_agent;
        private List<Vector3> m_patrolPoints;
        private int m_currentPatrolIndex = 0;
        private float m_patrolWaitTime = 2f;
        private float m_patrolTimer = 0f;
        private float m_arrivalDistance = 0.5f; // Adjust this value as needed
        private float m_idleTime = 3f; // Time to idle at each patrol point
        private float m_detectionRange = 10f; // Range to detect player
        private float m_attackRange = 2f; // Range to attack player
        private Transform m_playerTransform;
        private float m_sightRange = 10f;
        private float m_sightAngle = 90f;

        public Vector3? CurrentPatrolPoint => m_patrolPoints.Count > 0 ? m_patrolPoints[m_currentPatrolIndex] : null;


        public void Init(Enemy enemy, NavMeshAgent navAgent, Animator anim)
        {
            m_enemyReference = enemy;
            m_agent = navAgent;
            m_animator = anim;
            SetupPatrolPoints();
            SetupBehaviorTree();
        }

        public void Tick()
        {
            m_root.Evaluate();
        }

        void SetupBehaviorTree()
        {
            m_root = new Selector();

            // Attack sequence
            var attackSequence = new Sequence();
            attackSequence.AddChild(new ConditionNode(IsPlayerInAttackRange));
            attackSequence.AddChild(new ActionNode(PerformAttack));

            // Chase sequence
            var chaseSequence = new Sequence();
            chaseSequence.AddChild(new ConditionNode(IsPlayerInDetectionRange));
            chaseSequence.AddChild(new ActionNode(ChasePlayer));

            // Patrol sequence
            var patrolSequence = new Sequence();
            patrolSequence.AddChild(new ActionNode(Patrol));
            
            // Idle sequence
            var idleSequence = new Sequence();
            idleSequence.AddChild(new ConditionNode(ShouldIdle));
            idleSequence.AddChild(new ActionNode(PerformIdle));

            // Add all sequences to root selector in the correct order
            ((Selector)m_root).AddChild(attackSequence);
            ((Selector)m_root).AddChild(chaseSequence);
            ((Selector)m_root).AddChild(patrolSequence);
            ((Selector)m_root).AddChild(idleSequence);
        }

        // Condition methods
        bool IsPlayerInAttackRange()
        {
            // TODO: Check if player is within attackRange
            return false;
        }

        bool IsPlayerInDetectionRange()
        {
            // TODO: Check if player is within detectionRange but outside attackRange
            return false;
        }

        bool ShouldIdle()
        {
            // TODO: Check if we've reached a patrol point and should idle
            return false;
        }

        // Action methods
        NodeState PerformAttack()
        {
            // TODO: Implement attack logic
            m_animator.SetTrigger("Attack");
            Debug.Log("Performing attack");
            return NodeState.SUCCESS;
        }

        NodeState ChasePlayer()
        {
            // TODO: Implement chase logic using NavMeshAgent
            m_animator.SetBool("IsChasing", true);
            Debug.Log("Chasing player");
            return NodeState.RUNNING;
        }

        private NodeState Patrol()
        {
            if (m_patrolPoints.Count == 0)
            {
                Debug.LogWarning("No patrol points set up!");
                return NodeState.FAILURE;
            }

            Vector3 currentDestination = m_patrolPoints[m_currentPatrolIndex];
            float distanceToDestination = Vector3.Distance(m_agent.transform.position, currentDestination);

           // Debug.Log($"Current patrol index: {m_currentPatrolIndex}, Distance to destination: {distanceToDestination}");

            if (distanceToDestination <= m_arrivalDistance)
            {
                if (m_patrolTimer < m_patrolWaitTime)
                {
                    // Wait at the current point
                    m_patrolTimer += Time.deltaTime;
                    m_animator.SetFloat("Speed", 0);
                    //Debug.Log($"Waiting at point {m_currentPatrolIndex}. Timer: {m_patrolTimer}");
                    return NodeState.RUNNING;
                }
                else
                {
                    // Move to the next point
                    m_currentPatrolIndex = (m_currentPatrolIndex + 1) % m_patrolPoints.Count;
                    Vector3 nextDestination = m_patrolPoints[m_currentPatrolIndex];
                    m_agent.SetDestination(nextDestination);
                    m_patrolTimer = 0f;
                    //Debug.Log($"Moving to next point. New index: {m_currentPatrolIndex}, New destination: {nextDestination}");
                }
            }
            else if (!m_agent.pathPending && m_agent.remainingDistance < 0.1f)
            {
                // The agent thinks it has arrived, but our distance check disagrees so we force move to the exact position
                m_agent.Warp(currentDestination);
                //Debug.Log("Forced warp to destination due to small remaining distance");
            }

            // Move towards the current patrol point
            m_animator.SetFloat("Speed", m_agent.velocity.magnitude);
            return NodeState.RUNNING;
        }

        NodeState PerformIdle()
        {
            // TODO: Implement idle logic
            // - Start a coroutine to idle for idleTime seconds
            m_animator.SetBool("IsWalking", false);
            Debug.Log("Idling");
            return NodeState.RUNNING;
        }

        public void SetupPatrolPoints()
        {
            // TODO: Set up patrol points, either manually or procedurally
            m_patrolPoints = NavMeshPointHolder.Points;
            foreach (Vector3 point in m_patrolPoints)
            {
                //Debug.Log("NavMesh Point: " + point);
            }
            // Add patrol points...
        }

        private bool IsPlayerInSight()
        {
            if (m_playerTransform == null) return false;

            Vector3 directionToPlayer = m_playerTransform.position - m_enemyReference.transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            if (distanceToPlayer <= m_sightRange)
            {
                float angleToPlayer = Vector3.Angle(m_enemyReference.transform.forward, directionToPlayer);

                if (angleToPlayer <= m_sightAngle / 2)
                {
                    // Optional: Add a raycast check here for obstacles
                    return true;
                }
            }

            return false;
        }
    }
}
