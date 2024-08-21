using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSGJ15_DCSA.Interfaces;

namespace PSGJ15_DCSA
{
    public class HealthComponent : MonoBehaviour , IDamageable
    {
        private int m_currentHealth;
        private bool m_alreadyDying;
        readonly private string m_dissolvePropertyName = "_Dissolve";
        [SerializeField] private int m_maxHealth;
        [SerializeField] private float m_dissolveDuration = 2f;

        private MeshRenderer m_meshRenderer;

        void Awake()
        {
            m_currentHealth = m_maxHealth;
            m_meshRenderer = GetComponent<MeshRenderer>();
        }

        public void TakeDamage(int amount)
        {
            m_currentHealth -= amount;

            if(m_currentHealth <= 0 && !m_alreadyDying)
            { 
                m_alreadyDying = true;
                StartCoroutine(Death());
            }
        }

        private IEnumerator Death()
        {
            yield return StartCoroutine(DestructionAnimation());
            gameObject.SetActive(false);
            // add a pooling system here maybe?
        }

        private IEnumerator DestructionAnimation()
        {
            float dissolveValue = 0f;

            while (dissolveValue < 1f)
            {
                dissolveValue += Time.deltaTime / m_dissolveDuration;
                m_meshRenderer.material.SetFloat(m_dissolvePropertyName, dissolveValue);
                if (m_meshRenderer != null)
                {
                    m_meshRenderer.material.SetFloat(m_dissolvePropertyName, dissolveValue);
                }
                yield return null;
            }

            m_meshRenderer.material.SetFloat(m_dissolvePropertyName, 1f);
            if (m_meshRenderer != null)
            {
                m_meshRenderer.material.SetFloat(m_dissolvePropertyName, 1f);
            }
        }
    }
}
