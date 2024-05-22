using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class KeyValuePair<T1, T2>
{
    public T1 key;
    public T2 value;
}

public class AudioManager : MonoBehaviour
{
    // Serialized Fields
    // [SerializeField] AudioSource backgroundAudioSource = null;
    [SerializeField] List<KeyValuePair<string, AudioClip>> soundEffectDictionaryList = new List<KeyValuePair<string, AudioClip>>();

    // Variables
    Dictionary<string, AudioClip> soundEffectDictionary = new Dictionary<string, AudioClip>();

    void Awake()
    {
        foreach (KeyValuePair<string, AudioClip> kvp in soundEffectDictionaryList)
        {
            soundEffectDictionary[kvp.key] = kvp.value;
        }
    }

    public AudioSource PlayEffectAtLocation(Vector3 pos, float spatialBlend, float volume, string clipName)
    {
        GameObject tmpAudio = new GameObject("TmpAudio");
        tmpAudio.transform.position = pos;
        tmpAudio.transform.parent = transform;

        AudioSource audioSource = tmpAudio.AddComponent<AudioSource>();
        audioSource.spatialBlend = spatialBlend;
        audioSource.clip = soundEffectDictionary[clipName];
        audioSource.volume = volume;
        audioSource.Play();

        Destroy(tmpAudio, soundEffectDictionary[clipName].length);

        return audioSource;
    }
}
