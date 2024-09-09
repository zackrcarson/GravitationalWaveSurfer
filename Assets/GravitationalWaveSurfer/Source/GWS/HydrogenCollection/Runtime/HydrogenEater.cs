using UnityEngine;
using System;

namespace GWS.HydrogenCollection.Runtime
{
    /// <summary>
    /// Handles deleting hydrogen instances and adding hydrogen points.
    /// </summary>
    public class HydrogenEater : MonoBehaviour
    {
        public static HydrogenEater Instance { get; private set; }

        [SerializeField]
        private ParticleInventory particleInventory;
        
        [SerializeField]
        private AudioSource constantAudioSource;

        [SerializeField]
        private ParticleSystem sparks;

        [SerializeField]
        private AudioClip pop;

        [SerializeField]
        private AudioClip flash;

        [SerializeField]
        private float audioCooldown = 0.01f;

        private float lastAudioTime;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Electron"))
            {
                HandleCollision(other, pop);
                HydrogenManager.Instance.AddHydrogen(1);
                // AddHydrogen(1);
            }
            else if (other.CompareTag("Anti-Electron"))
            {
                HandleCollision(other, flash);
                HydrogenManager.Instance.AddHydrogen(-1);
                // AddHydrogen(-1);
            }
        }
        
        /// <summary>
        /// Called by HydrogenManager.AddHydrogen(), a (useless) wrapper function
        /// </summary>
        /// <param name="amount">ALREADY SCALED amount to be added</param>
        public void AddHydrogen(double amount)
        {
            particleInventory.HydrogenCount += amount;
            particleInventoryEventChannel.RaiseOnHydrogenCountChanged(particleInventory.HydrogenCount);
            // Debug.Log($"adding {amount} hydrogen");
        }

        private void HandleCollision(Component other, AudioClip clip)
        {
            BobCollection.attractedObjects.Remove(other.transform);
            Destroy(other.gameObject);

            if (constantAudioSource && Time.time >= lastAudioTime + audioCooldown)
            {
                constantAudioSource.pitch = UnityEngine.Random.Range(1f, 1.25f);
                constantAudioSource.PlayOneShot(clip, 0.1f);
                lastAudioTime = Time.time;
                sparks.Play();
            }
        }
    }
}
