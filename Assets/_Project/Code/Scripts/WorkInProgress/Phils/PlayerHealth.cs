using System.Collections;
using PSGJ15_DCSA.Core;
using UnityEngine;

namespace PSGJ15_DCSA
{
    public class PlayerHealth : HealthComponentBase
    {    
        private HUD m_HUD_Component;
        private float m_TimeSinceLastDamage;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            m_HUD_Component = GameManager.Instance.HUD_Component;
            m_HUD_Component.SetInitialHealth(m_currentHealth);
        }

        private void Update()
        {
            SimulateDamage();
        }

        public override void TakeDamage(int amount)
        {
            base.TakeDamage(amount);
            m_HUD_Component.UpdateHealthDisplay(amount);
        }

        private void SimulateDamage()
        {
            m_TimeSinceLastDamage += Time.deltaTime;
            if (m_TimeSinceLastDamage >= 1f)
            {
                TakeDamage(1);
                m_TimeSinceLastDamage = 0f;
            }
        }

        protected override IEnumerator Death()
        {
            // Disable player movement
            // GetComponent<PlayerMovement>().enabled = false;

            // Show death UI
            // GameManager.Instance.ShowDeathScreen();

            // Optionally, you might want to disable the player's collider or renderer
            // GetComponent<Collider>().enabled = false;
            // GetComponent<Renderer>().enabled = false;

            yield return StartCoroutine(PlayDeathSequence());

            // Optionally, restart the level or load a game over scene
            // SceneManager.LoadScene("GameOver");
        }

        protected override IEnumerator PlayDeathSequence()
        {
            // Play death animation
            // Animator animator = GetComponent<Animator>();
            // if (animator != null)
            // {
            //     animator.SetTrigger("Die");
            //     yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            // }

            // Play death sound
            // AudioSource audioSource = GetComponent<AudioSource>();
            // if (audioSource != null)
            // {
            //     audioSource.PlayOneShot(deathSound);
            //     yield return new WaitForSeconds(deathSound.length);
            // }

            yield return null;
        }
    }
}