using System.Collections;
using UnityEngine;
using PSGJ15_DCSA.Core.SimplifiedBehaviorTree;

namespace PSGJ15_DCSA
{
    public class EnnemyHealth : HealthComponentBase
    {
        [SerializeField] private GameObject m_hitVFXPrefab;
        [SerializeField] private AudioClip m_hitSFX;
        [SerializeField] private AudioClip m_deathSFX;

        private AudioSource m_audioSource;
        private Animator m_animator;

        // Reference to the behavior tree component
        private EnemyBehavior m_behaviorTree;

        protected override void Awake()
        {
            base.Awake();
            m_audioSource = GetComponent<AudioSource>();
            m_animator = GetComponent<Animator>();
            //m_behaviorTree = GetComponent<EnemyBehavior>();
        }

        public override void TakeDamage(int amount)
        {
            base.TakeDamage(amount);
            PlayHitEffects();
        }

        private void PlayHitEffects()
        {
            // Play hit VFX
            if (m_hitVFXPrefab != null)
            {
                Instantiate(m_hitVFXPrefab, transform.position, Quaternion.identity);
            }

            // Play hit SFX
            if (m_audioSource != null && m_hitSFX != null)
            {
                m_audioSource.PlayOneShot(m_hitSFX);
            }
        }

        protected override IEnumerator Death()
        {
            // Trigger death animation via behavior tree
            if (m_behaviorTree != null)
            {
                // m_behaviorTree.TriggerDeathAnimation();
            }

            // Play death SFX
            if (m_audioSource != null && m_deathSFX != null)
            {
                m_audioSource.PlayOneShot(m_deathSFX);
            }

            yield return StartCoroutine(PlayDeathSequence());

            // Note: If using object pooling, replace Destroy with returning to pool
            // ObjectPool.Instance.ReturnToPool(gameObject);
            Destroy(gameObject);
        }

        protected override IEnumerator PlayDeathSequence()
        {
            // Wait for death animation to complete
            if (m_animator != null)
            {
                // m_animator.SetTrigger("Die");
                // yield return new WaitForSeconds(m_animator.GetCurrentAnimatorStateInfo(0).length);
            }

            // Additional death effects or logic can be added here
            // For example, spawning loot, updating game state, etc.

            yield return null;
        }
    }
}