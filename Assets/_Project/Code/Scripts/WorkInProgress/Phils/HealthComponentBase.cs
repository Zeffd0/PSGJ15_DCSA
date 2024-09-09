using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSGJ15_DCSA.Interfaces;
using PSGJ15_DCSA.Enums;

namespace  PSGJ15_DCSA
{
    public abstract class HealthComponentBase : MonoBehaviour, IDamageable
    {
        protected int m_currentHealth;
        protected bool m_alreadyDying;
        [SerializeField] protected int m_maxHealth;

        // if it turns out that we won't even have VFXs on half
        // the stuff we put our healthcomponent on
        // we can move this down to a child
        [SerializeField] protected GameObject[] m_damageReceivedVFX;
        [SerializeField] protected GameObject[] m_deathVFX;
        protected bool m_hasVFX;
        protected ParticleSystem m_particleSystemCache;

        protected virtual void Awake()
        {
            m_currentHealth = m_maxHealth;
        }

        public virtual void TakeDamage(int amount)
        {
            m_currentHealth -= amount;
            HandleVFX(m_damageReceivedVFX);
            if (m_currentHealth <= 0 && !m_alreadyDying)
            {
                m_alreadyDying = true;
                StartCoroutine(Death());
            }
        }

        protected void HandleVFX(GameObject[] vfxArray)
        {
            if (vfxArray.Length > 0 && m_particleSystemCache == null)
            {
                GameObject vfxInstance = Instantiate(vfxArray[Random.Range(0, vfxArray.Length)], transform.position, Quaternion.identity);
                vfxInstance.transform.SetParent(transform, true);

                if (!vfxInstance.TryGetComponent(out m_particleSystemCache))
                {
                    Debug.LogWarning($"VFX prefab does not contain a ParticleSystem component.");
                }
            }
            else if (m_particleSystemCache != null)
            {
                m_particleSystemCache.Play();
            }
        }
        protected abstract IEnumerator Death();
        protected abstract IEnumerator PlayDeathSequence();

    }
}
