using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GWS.UI.Runtime
{
    public class GlossaryEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private AudioClip onEnterSound;

        [SerializeField]
        private AudioSource constantAudioSource;

        [SerializeField] 
        private float hoverScale = 1.1f;

        [SerializeField] 
        private float animationDuration = 0.1f;

        private Vector3 originalScale;
        private Coroutine scaleCoroutine;

        private void Start()
        {
            originalScale = transform.localScale;
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (constantAudioSource != null) constantAudioSource.PlayOneShot(onEnterSound, 0.8f);
            if (scaleCoroutine != null)
                StopCoroutine(scaleCoroutine);
            scaleCoroutine = StartCoroutine(ScaleAnimation(originalScale, originalScale * hoverScale));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (scaleCoroutine != null)
                StopCoroutine(scaleCoroutine);
            scaleCoroutine = StartCoroutine(ScaleAnimation(transform.localScale, originalScale));
        }

        private IEnumerator ScaleAnimation(Vector3 startScale, Vector3 endScale)
        {
            float elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float t = elapsedTime / animationDuration;

                // Quadratic easing
                t = t * t;

                transform.localScale = Vector3.Lerp(startScale, endScale, t);
                yield return null;
            }

            transform.localScale = endScale;
        }
    }
}
