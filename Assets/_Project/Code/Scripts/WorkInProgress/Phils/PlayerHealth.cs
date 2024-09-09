using System.Collections;
using UnityEngine;

namespace PSGJ15_DCSA
{
    public class PlayerHealth : HealthComponentBase
    {    
        // Reference to the UI element that displays player health
        [SerializeField] private UnityEngine.UI.Slider healthSlider;

        protected override void Awake()
        {
            base.Awake();
            // Initialize the health slider
            if (healthSlider != null)
            {
                healthSlider.maxValue = m_maxHealth;
                healthSlider.value = m_currentHealth;
            }
        }

        public override void TakeDamage(int amount)
        {
            base.TakeDamage(amount);
            UpdateHealthUI();
        }

        private void UpdateHealthUI()
        {
            // Update the UI element with the current health
            if (healthSlider != null)
            {
                healthSlider.value = m_currentHealth;
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