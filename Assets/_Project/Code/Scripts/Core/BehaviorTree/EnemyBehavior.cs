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
        private float m_patrolWaitTime = 3f;
        private float m_patrolTimer = 0f;
        private float m_arrivalDistance = 0.5f;
        private float m_detectionRange = 2f;
        private float m_attackRange = 1f;
        private Transform m_playerTransform;
        private float m_sightRange = 10f;
        private float m_sightAngle = 90f;
        private float m_rotationSpeed = 2f;
        // private float m_attackCooldown = 2f;
        // private float m_lastAttackTime = 0f;

        private enum PatrolState { Moving, Turning, Idling }
        private PatrolState m_currentPatrolState = PatrolState.Moving;

        public Vector3? CurrentPatrolPoint => m_patrolPoints.Count > 0 ? m_patrolPoints[m_currentPatrolIndex] : null;

        public void Init(Enemy enemy, NavMeshAgent navAgent, Animator anim)
        {
            m_enemyReference = enemy;
            m_agent = navAgent;
            m_animator = anim;
            SetupPatrolPoints();
            SetupBehaviorTree();
            //bandaid solution for now, until gamemanager is refactored properly
            m_playerTransform = (GameManager.Instance?.ReferenceToPlayer != null) ? GameManager.Instance.ReferenceToPlayer.transform : enemy.transform;
        }

        public void Tick()
        {
            m_root.Evaluate();
        }

        private void SetupBehaviorTree()
        {
            m_root = new Selector();

            // Attack sequence
            var attackSequence = new Sequence();
            attackSequence.AddChild(new ConditionNode(IsPlayerInAttackRange));
            attackSequence.AddChild(new ActionNode(PerformAttack));

            // Chase sequence
            var chaseSequence = new Sequence();
            chaseSequence.AddChild(new ConditionNode(IsPlayerInSight));
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
        #region Conditions
        private bool IsPlayerInAttackRange()
        {
            return Vector3.Distance(m_enemyReference.transform.position, m_playerTransform.position) <= m_attackRange;
        }

        private bool IsPlayerInSight()
        {
            if (m_playerTransform == null) return false;

            Vector3 directionToPlayer = m_playerTransform.position - m_enemyReference.transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            // Check if player is within attack range
            if (distanceToPlayer <= m_attackRange) return true;

            // Check if player is within detection range (for when player is behind the enemy)
            if (distanceToPlayer <= m_detectionRange) return true;

            // Check if player is within sight range and angle
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

        private bool ShouldIdle()
        {
            return m_currentPatrolState == PatrolState.Idling;
        }
        #endregion

        // Action methods
        #region Actions
        private NodeState PerformAttack()
        {
            m_agent.isStopped = true;
            m_animator.SetTrigger("StopAnimation");
            m_animator.SetBool("isChasing", false);
            m_animator.SetBool("isAttacking", true);
            //m_lastAttackTime = Time.time;

            Vector3 directionToPlayer = (m_playerTransform.position - m_enemyReference.transform.position).normalized;
            m_enemyReference.transform.rotation = Quaternion.LookRotation(directionToPlayer);

            //Debug.Log("Performing attack");
            return NodeState.RUNNING;
        }

        private NodeState ChasePlayer()
        {
            m_animator.SetTrigger("StopAnimation");
            m_animator.SetBool("isAttacking", false);
            m_animator.SetBool("isWalking", false);
            m_animator.SetBool("isIdling", false);
            m_animator.SetBool("isChasing", true);
            m_agent.SetDestination(m_playerTransform.position);

            return NodeState.RUNNING;
        }

        private NodeState Patrol()
        {
            m_animator.ResetTrigger("StopAnimation");
            if (m_patrolPoints.Count == 0)
            {
                Debug.LogWarning("No patrol points set up!");
                return NodeState.FAILURE;
            }

            m_animator.SetBool("isChasing", false);

            switch (m_currentPatrolState)
            {
                case PatrolState.Moving:
                    if (ReachedDestination())
                    {
                        m_currentPatrolState = PatrolState.Idling;
                        m_patrolTimer = 0f;
                        //Debug.Log($"Reached destination. Switching to Idle state.");
                    }
                    else
                    {
                        m_agent.isStopped = false;
                        m_agent.SetDestination(m_patrolPoints[m_currentPatrolIndex]);
                    }

                    m_animator.SetFloat("Speed", m_agent.velocity.magnitude);
                    m_animator.SetBool("isIdling", false);
                    m_animator.SetBool("isWalking", true);
                    break;

                case PatrolState.Idling:
                    m_agent.isStopped = true;
                    if (m_patrolTimer >= m_patrolWaitTime)
                    {
                        m_currentPatrolState = PatrolState.Turning;
                        SetNextPatrolPoint();
                        //Debug.Log($"Idle time complete. Switching to Turning state. New patrol index: {m_currentPatrolIndex}");
                    }
                    else
                    {
                        m_patrolTimer += Time.deltaTime;
                    }
                    m_animator.SetFloat("Speed", 0);
                    m_animator.SetBool("isWalking", false);
                    m_animator.SetBool("isIdling", true);
                    break;

                case PatrolState.Turning:
                    if (FacingNextPoint())
                    {
                        m_currentPatrolState = PatrolState.Moving;
                        //Debug.Log("Facing next point. Switching to Moving state.");
                    }
                    else
                    {
                        TurnTowardsNextPoint();
                    }
                    m_animator.SetBool("isWalking", false);
                    // look for a turning animation and insert here
                    break;
            }
            return NodeState.RUNNING;
        }

        private NodeState PerformIdle()
        {
            m_animator.SetBool("IsWalking", false);
            Debug.Log("Idling");
            return NodeState.RUNNING;
        }

        #endregion

        public void SetupPatrolPoints()
        {
            // When we get the singleton data holder, the class that's doing it should be replaced and we just fetch it from there.
            // Could pimp up this function or if it's still a 1 liner we can just put this line
            m_patrolPoints = NavMeshPointHolder.Points;
            // foreach (Vector3 point in m_patrolPoints)
            // {
            //     //Debug.Log("NavMesh Point: " + point);
            // }
        }

        #region Functions

        private bool ReachedDestination()
        {
            return Vector3.Distance(m_agent.transform.position, m_patrolPoints[m_currentPatrolIndex]) <= m_arrivalDistance;
        }

        private void SetNextPatrolPoint()
        {
            m_currentPatrolIndex = (m_currentPatrolIndex + 1) % m_patrolPoints.Count;
            m_agent.SetDestination(m_patrolPoints[m_currentPatrolIndex]);
        }

        private bool FacingNextPoint()
        {
            Vector3 directionToTarget = (m_patrolPoints[m_currentPatrolIndex] - m_agent.transform.position).normalized;
            float dot = Vector3.Dot(m_agent.transform.forward, directionToTarget);
            return dot > 0.95f; // Roughly within 18 degrees
        }

        private void TurnTowardsNextPoint()
        {
            Vector3 directionToTarget = (m_patrolPoints[m_currentPatrolIndex] - m_agent.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            m_agent.transform.rotation = Quaternion.Slerp(m_agent.transform.rotation, targetRotation, Time.deltaTime * m_rotationSpeed);
        }
        #endregion
    }
}
