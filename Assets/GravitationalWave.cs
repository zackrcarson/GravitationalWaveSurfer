using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalWave : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float amplitude = 0.1f;
    [SerializeField] float mass1 = 2;
    [SerializeField] float mass2 = 3;
    [SerializeField] float coalescenceTime = 0f;
    [SerializeField] float coalescencePhase = 0f;

    // Cached References
    const float solarMassToSeconds = 0.000005f;
    float chirpMass, totalMass, symMassRatio;
    float time = 0;
    float hOfT = 0;

    // Start is called before the first frame update
    void Start()
    {
        GetChirpMass();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        hOfT = Waveform(time);

        Debug.Log(hOfT);
    }

    private void GetChirpMass()
    {
        totalMass = mass1 + mass2;
        symMassRatio = (mass1 * mass2) / Mathf.Pow(totalMass, 2);
        chirpMass = totalMass * Mathf.Pow(symMassRatio, (3/5)) * solarMassToSeconds;
    }

    private float Waveform(float t)
    {
        float waveform = amplitude * Mathf.Cos(Phase(t));

        return waveform;
    }

    private float Phase(float t)
    {
        float phase =  -2 * Mathf.Pow( (1 / (5 * chirpMass)) * (coalescenceTime - t), (5 / 8)) + coalescencePhase;

        return phase;
    }
}
