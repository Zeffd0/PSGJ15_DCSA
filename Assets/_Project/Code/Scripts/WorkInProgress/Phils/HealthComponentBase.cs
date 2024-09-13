using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSGJ15_DCSA.Interfaces;
using PSGJ15_DCSA.Enums;
using TMPro;
using PrimeTween;

namespace  PSGJ15_DCSA
{
    public abstract class HealthComponentBase : MonoBehaviour, IDamageable
    {
        protected int m_currentHealth;
        protected bool m_alreadyDying;
        [SerializeField] protected int m_maxHealth;
        [SerializeField] protected GameObject[] m_damageReceivedVFX;
        [SerializeField] protected GameObject[] m_deathVFX;
        [SerializeField] protected GameObject m_damageTextPrefab;
        [SerializeField] protected float m_popUpDistance = 1f;
        [SerializeField] protected float m_popUpDuration = 0.5f;
        [SerializeField] protected float m_fadeOutDuration = 0.3f;
        [SerializeField] protected float m_lingerDuration = 0.5f;
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

        protected void ShowDamage(int damageAmount)
        {
            GameObject damageTextObject = Instantiate(m_damageTextPrefab, transform.position, Quaternion.identity, transform);
            TextMeshPro damageText = damageTextObject.GetComponent<TextMeshPro>();
            
            if (damageText != null)
            {
                damageText.text = damageAmount.ToString();
                AnimateDamageText(damageTextObject, damageText);
            }
        }
        private void AnimateDamageText(GameObject textObject, TextMeshPro textMesh)
        {
            Vector3 startPosition = textObject.transform.localPosition;
            Vector3 endPosition = startPosition + Vector3.up * m_popUpDistance;

            // Sequence of animations
            Sequence.Create()
                // 1. Pop up animation
                .Chain(Tween.LocalPosition(textObject.transform, endValue: endPosition, duration: m_popUpDuration, ease: Ease.OutBack))
                
                // 2. Punch effect (starts halfway through the pop-up)
                .Group(Tween.Delay(m_popUpDuration * 0.5f, () => 
                    Tween.PunchLocalPosition(textObject.transform, strength: Vector3.up * 0.2f, duration: 0.3f, frequency: 10)))
                
                // 3. Linger delay
                .Chain(Tween.Delay(m_lingerDuration))
                
                // 4. Fade out
                .Chain(Tween.Alpha(textMesh, startValue: 1f, endValue: 0f, duration: m_fadeOutDuration))
                
                // 5. Destroy the object
                .Chain(Tween.Delay(0.1f, () => Destroy(textObject)));
        }
        protected abstract IEnumerator Death();
        protected abstract IEnumerator PlayDeathSequence();
    }
}
