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

    [SerializeField]
    private AudioClip flash;

    private float audioCooldown = 0.01f;

    private float lastAudioTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Electron"))
        {
            HandleCollision(other, pop);
            HydrogenTracker.Instance.AddHydrogen(100);
        }
        else if (other.CompareTag("Anti-Electron"))
        {
            HandleCollision(other, flash);
            HydrogenTracker.Instance.AddHydrogen(-1);
        }
    }

    private void HandleCollision(Collider other, AudioClip clip)
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
