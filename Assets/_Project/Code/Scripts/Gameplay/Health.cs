using System.Collections;
using System.Collections.Generic;
using PSGJ15_DCSA.Core.DependencyAgents;
using PSGJ15_DCSA.Enums;
using PSGJ15_DCSA.Interfaces;
using UnityEngine;

namespace PSGJ15_DCSA.Gameplay
{
    public class Health : MonoBehaviour, IHealth, IDefense
    {
        [Header("Dependency Agents")]
        [SerializeField] private DA_GameStates m_DAGameStates;
        [SerializeField] private DA_CharacterEvents m_DACharacterEvents;

        [Header("Health Script Fields")]
        [SerializeField] private STAT_Character m_characterStats;
        [SerializeField] private bool m_isDead;
        [SerializeField] private bool m_isImmune;

        private void Awake()
        {
            m_isDead = false;
            m_isImmune = false;
            m_characterStats.Initialize();

            m_DAGameStates = (DA_GameStates)REG_DependencyAgents.Instance.GetDependencyAgent(DependencyAgentType.GameState);
            m_DACharacterEvents = (DA_CharacterEvents)REG_DependencyAgents.Instance.GetDependencyAgent(DependencyAgentType.Character);

            // GetComponents
            // GetDependencyAgents
        }

        public void ReceiveDamage(int damage, ElementTypes elementType)
        {
            Debug.Log("I AM BEING TRIGGERED");
            if(elementType != ElementTypes.None && elementType == m_characterStats.GetElementAffinity())
            {
                // TODO: Pop up text that shows "Immune or turn this into a heal.
                Debug.Log("SAME ELEMENT AFFINITY!");
                return;
            }
            else
            {
                m_DACharacterEvents.InvokeCharacterGettingHit(this, gameObject, m_characterStats);
                ApplyDamage(damage);
            }
        }

        private void ApplyDamage(int damage)
        {
            Debug.Log("APPLYING DAMAGE!");
            int currentHealth = m_characterStats.GetHealthValue();
            if(currentHealth - damage <= 0)
            {
                Die();
            }
            else
            {
                Debug.Log("YOU RECEIVE " + damage + " DAMAGE!");
                m_characterStats.SetHealthValue(currentHealth - damage);
            }
        }

        private void Die()
        {
            Debug.Log("YOU ARE DED!");
            m_isDead = true;
            m_characterStats.SetHealthValue(0);
        }
    }
}
