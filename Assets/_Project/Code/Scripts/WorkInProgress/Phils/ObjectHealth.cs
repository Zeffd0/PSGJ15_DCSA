using System.Collections;
using UnityEngine;

namespace  PSGJ15_DCSA
{
    public class ObjectHealth : HealthComponentBase
    {    
        [SerializeField] private float m_dissolveDuration = 2f;
        [SerializeField] private MeshRenderer m_meshRenderer;
        private readonly string m_dissolvePropertyName = "_Dissolve";
        private bool m_canDissolve;

        private void Start()
        {
            if (m_meshRenderer == null) m_meshRenderer = GetComponent<MeshRenderer>();
            if (m_meshRenderer != null)
            m_canDissolve = m_meshRenderer.material.HasProperty(m_dissolvePropertyName);
        }

        protected override IEnumerator Death()
        {
            if (m_meshRenderer != null && m_meshRenderer.material.HasProperty(m_dissolvePropertyName))
            {
                yield return StartCoroutine(PlayDeathSequence());
            }
            HandleVFX(m_deathVFX);  // Play death VFX
            gameObject.SetActive(false);
        }

        protected override IEnumerator PlayDeathSequence()
        {
            float dissolveValue = 0f;
            while (dissolveValue < 1f)
            {
                dissolveValue += Time.deltaTime / m_dissolveDuration;
                m_meshRenderer.material.SetFloat(m_dissolvePropertyName, dissolveValue);
                yield return null;
            }
            m_meshRenderer.material.SetFloat(m_dissolvePropertyName, 1f);
        }
    }
}
