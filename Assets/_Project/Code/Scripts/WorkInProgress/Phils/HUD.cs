using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;

namespace PSGJ15_DCSA
{
    public class HUD : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text m_HealthText;
        [SerializeField] private Image m_HealthBarDamageTaken;
        [SerializeField] private Image m_HealthBarHealthRemaining;
        [SerializeField] private TMP_Text m_ScoreText;
        [SerializeField] private TMP_Text m_TimerText;

        private int m_MaxHealth;
        private int m_Score = 0;
        private float m_Timer = 0f;

        private void Start()
        {
        }

        private void Update()
        {
            //UpdateTimer();
        }

        private void TestTween()
        {

        }

        private void UpdateScoreDisplay()
        {
            m_ScoreText.text = $"Score: {m_Score}";
        }

        private void UpdateTimer()
        {
            m_Timer += Time.deltaTime;
            int minutes = Mathf.FloorToInt(m_Timer / 60f);
            int seconds = Mathf.FloorToInt(m_Timer % 60f);
            m_TimerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }

        public void AddScore(int points)
        {
            m_Score += points;
            UpdateScoreDisplay();
        }

        public void SetInitialHealth(int health)
        {
            UpdateHealthText(health);
            m_MaxHealth = health;
            m_previousHealth = m_MaxHealth;
            m_HealthBarHealthRemaining.fillAmount = 1f;
        }

        private int m_previousHealth;

        public void UpdateHealthDisplay(int damageTaken)
        {
            float healthBeforeDamage = m_previousHealth;
            m_previousHealth -= damageTaken;
            UpdateHealthText(m_previousHealth);
            UpdateHealthBar(m_previousHealth, healthBeforeDamage);
        }

        private void UpdateHealthText(int health)
        {
            m_HealthText.text = health.ToString();
        }

        private void UpdateHealthBar(float currentHealth, float previousHealth)
        {
            float healthPercentage = currentHealth / m_MaxHealth;
            m_HealthBarHealthRemaining.fillAmount = healthPercentage;
            StartCoroutine(AnimateDamageBar(currentHealth, previousHealth));
        }

        private IEnumerator AnimateDamageBar(float currentHealth, float previousHealth)
        {
            float animationDuration = 2.5f;
            float elapsedTime = 0f;
            float startHealth = previousHealth / m_MaxHealth;
            float targetHealth = currentHealth / m_MaxHealth;

            while (elapsedTime < animationDuration)
            {
                m_HealthBarDamageTaken.fillAmount = Mathf.Lerp(startHealth, targetHealth, elapsedTime / animationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            m_HealthBarDamageTaken.fillAmount = targetHealth;
        }
    }
}