using GWS.Gameplay;
using GWS.Player;
using UnityEngine;

/// <summary>
/// Handles deleting hydrogen instances and adding hydrogen points.
/// </summary>
public class HydrogenEater : MonoBehaviour
{
    [SerializeField]
    private AudioSource constantAudioSource;

    [SerializeField]
    private ParticleSystem sparks;

    [SerializeField]
    private AudioClip pop;

    private float audioCooldown = 0.01f;

    private float lastAudioTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Electron"))
        {
            BobCollection.attractedObjects.Remove(other.transform);
            Destroy(other.gameObject);
            HydrogenTracker.Instance.AddHydrogen(1);

            if (Time.time >= lastAudioTime + audioCooldown)
            {
                constantAudioSource.pitch = Random.Range(1f, 1.25f);
                constantAudioSource.PlayOneShot(pop, 0.1f);
                lastAudioTime = Time.time;
                sparks.Play();
            }
        }
    }
}
