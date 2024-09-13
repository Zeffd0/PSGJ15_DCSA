using UnityEngine;
using TMPro;
using PrimeTween;

namespace PSGJ15_DCSA
{
    public class DamagePopup : MonoBehaviour
    {
        public GameObject damageTextPrefab;
        public float popUpDistance = 1f;
        public float popUpDuration = 0.5f;
        public float fadeOutDuration = 0.3f;
        public float lingerDuration = 0.5f; // Time to wait before fading out


        private float lastPopupTime;
        public float popupInterval = 2f; // Interval between automatic popups for testing

        private void Update()
        {
            // Test popup every 2 seconds
            if (Time.time - lastPopupTime >= popupInterval)
            {
                ShowDamage(Random.Range(1, 100));
                lastPopupTime = Time.time;
            }
        }

        public void ShowDamage(int damageAmount)
        {
            GameObject damageTextObject = Instantiate(damageTextPrefab, transform.position, Quaternion.identity, transform);
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
            Vector3 endPosition = startPosition + Vector3.up * popUpDistance;

            // Sequence of animations
            Sequence.Create()
                // 1. Pop up animation
                .Chain(Tween.LocalPosition(textObject.transform, endValue: endPosition, duration: popUpDuration, ease: Ease.OutBack))
                
                // 2. Punch effect (starts halfway through the pop-up)
                .Group(Tween.Delay(popUpDuration * 0.5f, () => 
                    Tween.PunchLocalPosition(textObject.transform, strength: Vector3.up * 0.2f, duration: 0.3f, frequency: 10)))
                
                // 3. Linger delay
                .Chain(Tween.Delay(lingerDuration))
                
                // 4. Fade out
                .Chain(Tween.Alpha(textMesh, startValue: 1f, endValue: 0f, duration: fadeOutDuration))
                
                // 5. Destroy the object
                .Chain(Tween.Delay(0.1f, () => Destroy(textObject)));
        }
    }
}
