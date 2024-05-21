using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;

public class GravitationalWave : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float phaseFactor = 0.01f;
    [SerializeField] public float coalescenceTime = 10f;

    // Cached References
    float mass1 = 2.0f;
    float mass2 = 3.0f;

    float chirpMass, totalMass, symMassRatio;
    float time = 0.0f;
    float hOfT = 0.0f;

    // constants
    const float solarMassToSeconds = 0.000005f;

    // Start is called before the first frame update
    public void StartNewWave(float m1, float m2)
    {
        mass1 = m1;
        mass2 = m2;

        time = 0;

        GetChirpMass();
    }

    public float GetMaxAmplitude()
    {
        GetChirpMass();

        return Waveform(coalescenceTime + Time.deltaTime);
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

    // Wolfram: plot( (0.1 * Re((1 / (8 * pi * 2.5e-5)) * ((t) / (5 * 2.5e-5)) ^(-(3.0 / 8.0))))^(7/6)  * cos(.02 * Re(-2 * ( (1 / (5 * 2.5e-5)) * ( t))^(5.0 / 8.0))) ), t=0...10
    private float Waveform(float t)
    {
        float waveform = Amplitude(coalescenceTime - t) * Mathf.Cos(PhaseFactor() * Phase(coalescenceTime - t));

        return waveform;
    }

    private float PhaseFactor()
    {
        return phaseFactor * Mathf.Pow(chirpMass / 0.0000208932f , 4.66f / 8.0f);
    }

    private float Phase(float t)
    {
        Complex phase =  -2 * Complex.Pow( (1 / (5 * chirpMass)) * (t), (5.0 / 8.0));

        return (float)phase.Real;
    }

    private float Amplitude(float t)
    {
        Complex amplitude = Mathf.Sqrt(5.0f / 24.0f) * (1 / Mathf.Pow(Mathf.PI, 2.0f / 3.0f)) * Mathf.Pow(chirpMass, 5.0f / 6.0f) * Complex.Pow((1 / (8 * Mathf.PI * chirpMass)) * Complex.Pow((t) / (5 * chirpMass) , -(3.0 / 8.0)), (7.0 / 6.0));

        return (float)amplitude.Real;
    }
}
