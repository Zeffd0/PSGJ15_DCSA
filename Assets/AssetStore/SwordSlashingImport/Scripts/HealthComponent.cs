using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSGJ15_DCSA.Interfaces;
using PSGJ15_DCSA.Enums;

namespace PSGJ15_DCSA
{
    public class HealthComponent : MonoBehaviour , IDamageable
    {
        private int m_currentHealth;
        private bool m_alreadyDying;
        readonly private string m_dissolvePropertyName = "_Dissolve";
        [SerializeField] private int m_maxHealth;
        [SerializeField] private float m_dissolveDuration = 2f;
        [SerializeField] private DamageableType damageableType = DamageableType.None;
        [SerializeField] private MeshRenderer m_meshRenderer;
        [SerializeField] private AudioSource m_audioSource; // temporary
        [SerializeField] private GameObject[] m_damageReceivedVFX;
        [SerializeField] private GameObject[] m_breakingVFX;
        [SerializeField] private Animator m_deathAnimation;
        private ParticleSystem m_damageReceivedParticleSystem;
        private ParticleSystem m_breakingParticleSystem;
        private bool m_canDissolve;
        private bool m_hasAudioSource;
        private bool m_hasDeathAnimation;
        private bool m_hasMeshRenderer;

        private void Awake()
        {
            m_currentHealth = m_maxHealth;
            if (m_meshRenderer == null) m_meshRenderer = GetComponent<MeshRenderer>();
            if (m_audioSource == null)m_audioSource = GetComponent<AudioSource>();
            if (m_deathAnimation == null)m_deathAnimation = GetComponent<Animator>();
            
            if (m_meshRenderer != null)
            m_canDissolve = m_meshRenderer.material.HasProperty(m_dissolvePropertyName);
            m_hasMeshRenderer = m_meshRenderer != null;
            m_hasAudioSource = m_audioSource != null;
            m_hasDeathAnimation = m_deathAnimation != null && m_deathAnimation.HasState(0, Animator.StringToHash("Death"));
        }

        public void TakeDamage(int amount)
        {
            m_currentHealth -= amount;

            HandleVFX(m_damageReceivedVFX , m_damageReceivedParticleSystem);
            if (m_currentHealth <= 0 && !m_alreadyDying)
            {
                m_alreadyDying = true;
                StartCoroutine(Death(damageableType));
            }
        }

        private IEnumerator Death(DamageableType type)
        {
            switch(type)
            {
                case DamageableType.Player:
                    yield return StartCoroutine(PlayerDeathSequence());
                    break;
                case DamageableType.Ennemy:
                    yield return StartCoroutine(EnnemyDeathSequence());
                    break;
                case DamageableType.DestructibleObjects:
                    if(m_canDissolve)
                    {
                        yield return StartCoroutine(ObjectDeathSequence());   
                    }
                    HandleVFX(m_breakingVFX, m_breakingParticleSystem);
                    break;
                case DamageableType.LootBox:
                    //Todo:: I think by composition, perhaps even a scriptableobject to trigger some loot mechanic
                    break;
                case DamageableType.None:
                    //Todo:: maybe just add error notices in case we forget to set up the health component properly
                    break;
            }
            gameObject.SetActive(false);
        }

        private IEnumerator PlayerDeathSequence()
        {
            // Todo:: own player's consequences for dying, -1 life, UI change, respawn, unclear yet
            Debug.Log("Player dying");
            yield return null;
        }

        private IEnumerator EnnemyDeathSequence()
        {
            // Todo:: I think we can simply ping the state for Statemachine here..
            Debug.Log("Ennemy Dying");
            yield return null;
        }

        private IEnumerator ObjectDeathSequence()
        {
            Debug.Log("Object Dying");

            float dissolveValue = 0f;
            while (dissolveValue < 1f)
            {
                dissolveValue += Time.deltaTime / m_dissolveDuration;
                m_meshRenderer.material.SetFloat(m_dissolvePropertyName, dissolveValue);
                yield return null;
            }
            m_meshRenderer.material.SetFloat(m_dissolvePropertyName, 1f);
        }

        private void HandleVFX(GameObject[] vfxArray, ParticleSystem particleSystem)
        {
            if (vfxArray.Length > 0 && particleSystem == null)
            {
                GameObject vfxInstance = Instantiate(vfxArray[Random.Range(0, vfxArray.Length)], transform.position, Quaternion.identity);
                if (!vfxInstance.TryGetComponent(out particleSystem))
                {
                    Debug.LogWarning($"VFX prefab does not contain a ParticleSystem component.");
                }
            }
            else if (particleSystem != null)
            {
                particleSystem.Play();
            }
        }
    }
}
