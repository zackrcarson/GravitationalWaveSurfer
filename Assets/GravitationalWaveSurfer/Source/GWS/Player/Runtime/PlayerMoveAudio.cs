using GWS.Player.Runtime;
using GWS.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using GWS.SceneManagement.Runtime;
using UnityEngine;

namespace GWS.Player.Runtime
{
    /// <summary>
    /// Plays the humming sound whenever the player moves.
    /// </summary>
    public class PlayerMoveAudio: MonoBehaviour
    {
        /// <summary>
        /// The audio source that changes its volume based off the player's speed.
        /// </summary>
        [SerializeField]
        private AudioSource audioSource;

        /// <summary>
        /// The point at which the humming noise will stop if the magnitude of the velocity is lower than this.
        /// </summary>
        private float speedThreshold = 0.5f;

        /// <summary>
        /// The duration over which the humming noise will fade.
        /// </summary>
        private float fadeDuration = 0.15f;

        private float maxVolume = 0.8f;

        private int volumeDenominator = 50;

        private Coroutine fadeCoroutine;

        private void HandleSceneChange(bool state)
        {
            maxVolume = state ? 0.8f : 0f;
        }

        private void Start()
        {
            audioSource.volume = 0f;
            audioSource.Play();
        }

        private void OnEnable()
        {
            PlayerSpeedManager.OnSpeedChanged += HandleMoveSound;
            AdditiveSceneManager.OnChangeOfScene += HandleSceneChange;
        }

        private void OnDisable()
        {
            PlayerSpeedManager.OnSpeedChanged -= HandleMoveSound;
            AdditiveSceneManager.OnChangeOfScene -= HandleSceneChange;
        }

        private void HandleMoveSound(float magnitude)
        {
            if (magnitude > speedThreshold)
            {
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                audioSource.volume = Mathf.Clamp(magnitude / volumeDenominator, 0f, maxVolume);
            }
            else
            {
                fadeCoroutine = StartCoroutine(FadeOut(fadeDuration));
            }
        }

        private IEnumerator FadeOut(float duration)
        {
            float startingVolume = audioSource.volume;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(startingVolume, 0, t / duration);
                yield return null;
            }

            audioSource.volume = 0;
        }
    }
}
