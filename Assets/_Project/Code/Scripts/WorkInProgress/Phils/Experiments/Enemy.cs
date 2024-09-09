using PSGJ15_DCSA.Core;
using PSGJ15_DCSA.Core.SimplifiedBehaviorTree;
using PSGJ15_DCSA.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace  PSGJ15_DCSA
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Animator m_animator;
        [SerializeField] private NavMeshAgent m_agent;
        [SerializeField] private HealthComponent m_healthComponent;
        private EnemyBehavior m_enemyBehavior;

        private void Start()
        {
            if (m_agent == null) m_agent = GetComponent<NavMeshAgent>();
            if (m_healthComponent == null)  m_healthComponent = GetComponent<HealthComponent>();
            m_agent.updatePosition = false;
            m_enemyBehavior = new EnemyBehavior();
            m_enemyBehavior.Init(this, m_agent, m_animator);
        }

        private void Update()
        {
            m_enemyBehavior.Tick();
        }

        public void OnAttackAnimationHit()
        {
            float sphereCastRadius = 0.5f;
            float attackRange = 2f;
            LayerMask targetLayers = LayerMask.GetMask("Player"); 

            Transform attackOrigin = transform; // Change this if you have a specific point to cast from

            if (Physics.SphereCast(attackOrigin.position, sphereCastRadius, attackOrigin.forward, out RaycastHit hit, attackRange, targetLayers))
            {
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(1);
                    Debug.Log($"Hit {hit.collider.gameObject.name} and dealt {1} damage!");
                }
            }
            else
            {
                Debug.Log("Attack animation hit point reached, but no target was hit.");
            }
        }

        private void OnAnimatorMove()
        {
            Vector3 rootPosition = m_animator.rootPosition;
            m_agent.nextPosition = rootPosition;
            transform.position = rootPosition;
        }

        private void OnDrawGizmos()
        {
            if (m_agent == null || m_enemyBehavior == null) return;

            // Draw the path
            if (m_agent.hasPath)
            {
                Gizmos.color = Color.yellow;
                var path = m_agent.path;
                Vector3 previousCorner = transform.position;
                foreach (var corner in path.corners)
                {
                    Gizmos.DrawLine(previousCorner, corner);
                    previousCorner = corner;
                }
            }

            // Draw the current destination
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_agent.destination, 0.1f);

            // Draw the current patrol point
            if (m_enemyBehavior.CurrentPatrolPoint.HasValue)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(m_enemyBehavior.CurrentPatrolPoint.Value, 0.1f);
            }
        }

    }

}
