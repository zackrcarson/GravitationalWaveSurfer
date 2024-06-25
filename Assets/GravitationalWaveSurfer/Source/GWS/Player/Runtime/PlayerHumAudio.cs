using System.Collections;
using UnityEngine;
using GWS.GeneralRelativitySimulation.Runtime;

namespace GWS.Player.Runtime
{ 
    /// <summary>
    /// Manages sound and visual effects related to the player's speed.
    /// </summary>
    public class PlayerSpeedManager: MonoBehaviour
    {
        /// <summary>
        /// The reference to the player's rigidbody.
        /// </summary>
        [SerializeField]
        private new Rigidbody rigidbody;

        /// <summary>
        /// The reference to the player's rotation script that changes based off the player's speed.
        /// </summary>
        [SerializeField]
        private RotationalBehavior rotationalBehavior;

        /// <summary>
        /// The audio source that changes its volume based off the player's speed.
        /// </summary>
        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private float rotationalSpeed;

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
        private void Start()
        {
            audioSource.volume = 0f;
            audioSource.Play();
        }
        private void Update()
        {
            if (rigidbody.velocity.magnitude > speedThreshold)
            {
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                audioSource.volume = Mathf.Clamp(rigidbody.velocity.magnitude / volumeDenominator, 0f, maxVolume);
            }
            else
            {
                fadeCoroutine = StartCoroutine(FadeOut(fadeDuration));
            }

            rotationalBehavior.rotationDelta = new Vector3(0, rigidbody.velocity.magnitude * rotationalSpeed, 0);
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
        /*
        [SerializeField]
        private InputEventChannel inputEventChannel;

        private AudioSource audioSource;

        private bool isMoving = false;
        /// <summary>
        /// The time it takes for audio to fade
        /// </summary>
        private float fadeSeconds = 0.1f;

        private Coroutine fadeCoroutine;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            inputEventChannel.OnMove += HandleMove;
        }

        private void OnDisable()
        {
            inputEventChannel.OnMove -= HandleMove;
        }

        private void HandleMove(Vector2 position)
        {
            if (position != Vector2.zero && !isMoving)
            {
                isMoving = true;
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }   
                PlayHum();
            }
            else if (position == Vector2.zero && isMoving)
            {
                isMoving = false;
                fadeCoroutine = StartCoroutine(FadeOut(fadeSeconds));
            }
        }

        private void PlayHum()
        {
            if (!audioSource.isPlaying)
            {
                // Reset to default volume
                audioSource.volume = 1f; 
                audioSource.Play();
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
            audioSource.Stop();
        }
        */
    }

}
