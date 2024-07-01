using GWS.Input.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWS.Player.Runtime
{ 
    /// <summary>
    /// Plays pop sound when player pressed the interact button (E).
    /// </summary>
    public class PlayerInteractAudio: MonoBehaviour
    {
        [SerializeField]
        private InputEventChannel inputEventChannel;

        /// <summary>
        /// The audio source which will not change volume.
        /// </summary>
        [SerializeField]
        private AudioSource constantAudioSource;

        [SerializeField]
        private AudioClip popSound;

        private void OnEnable()
        {
            inputEventChannel.OnInteract += PlayPop;
        }

        private void OnDisable()
        {
            inputEventChannel.OnInteract -= PlayPop;
        }
        public void PlayPop()
        {
            constantAudioSource.PlayOneShot(popSound, 1f);
        }
    }
}
