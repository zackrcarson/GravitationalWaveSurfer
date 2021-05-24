using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;

public class GravitationalWave : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float mass1 = 2;
    [SerializeField] float mass2 = 3;
    [SerializeField] float phaseFactor = 0.01f;

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

    public float GetMaxAmplitude()
    {
        GetChirpMass();

        return Waveform(Time.deltaTime);
    }

    // Update is called once per frame
    public float GetGravitationalWave()
    {
        time += Time.deltaTime;

        hOfT = Waveform(time);

        return hOfT;
    }

    private void GetChirpMass()
    {
        totalMass = mass1 + mass2;
        symMassRatio = (mass1 * mass2) / Mathf.Pow(totalMass, 2);
        chirpMass = totalMass * Mathf.Pow(symMassRatio, (3/5)) * solarMassToSeconds;
    }

    private float Waveform(float t)
    {
        float waveform = Amplitude(t) * Mathf.Cos(phaseFactor * Phase(t));

        return waveform;
    }

    private float Phase(float t)
    {
        Complex phase =  -2 * Complex.Pow( (1 / (5 * chirpMass)) * (t), (5.0 / 8.0));

        return (float)phase.Real;
    }

    private float Amplitude(float t)
    {
        Complex amplitude = (1 / (8 * Mathf.PI * chirpMass)) * Complex.Pow((t) / (5 * chirpMass) , -(3.0 / 8.0));

        return (float)amplitude.Real;
    }
}
