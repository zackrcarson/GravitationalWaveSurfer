using UnityEngine;

namespace GWS.HydrogenCollection.Runtime
{
    /// <summary>
    /// Handles deleting hydrogen instances and adding hydrogen points.
    /// </summary>
    public class HydrogenEater : MonoBehaviour
    {
        [SerializeField]
        private ParticleInventory particleInventory;
        
        [SerializeField] 
        private ParticleInventoryEventChannel particleInventoryEventChannel;
        
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Electron"))
            {
                HandleCollision(other, pop);
                AddHydrogen(1);
            }
            else if (other.CompareTag("Anti-Electron"))
            {
                HandleCollision(other, flash);
                AddHydrogen(-1);
            }
        }
        
        private void AddHydrogen(int amount)
        {
            particleInventory.HydrogenCount += amount;
            particleInventoryEventChannel.RaiseOnHydrogenCountChanged(particleInventory.HydrogenCount);
            Debug.Log("adding 1 hydrogen");
        }

        private void HandleCollision(Component other, AudioClip clip)
        {
            BobCollection.attractedObjects.Remove(other.transform);
            Destroy(other.gameObject);

            if (Time.time >= lastAudioTime + audioCooldown)
            {
                constantAudioSource.pitch = Random.Range(1f, 1.25f);
                constantAudioSource.PlayOneShot(clip, 0.1f);
                lastAudioTime = Time.time;
                sparks.Play();
            }
        }
    }
}
