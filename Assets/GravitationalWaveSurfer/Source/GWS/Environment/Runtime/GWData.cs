using UnityEngine;

namespace GWS.Data
{
    /// <summary>
    /// Information about the gravitational wave for simulation
    /// </summary>
    public class GWData
    {
        public Vector3 sourcePosition;
        public Vector3 destinationPosition;
        public float initialFrequency = 1f; // Initial frequency of the gravitational wave
        public float initialAmplitude = 1f; // Amplitude of the gravitational wave
        public float mergeTime = 5f; // Time at which the merge occurs
        public float peakFrequency = 1f; // Maximum frequency
        public float peakAmplitude = 2f; // Maximum amplitude
        public float postMergerDecayRate = 2f; // Decay rate after merge

        public GWData(Vector3 sourcePos, Vector3 destPos,
                      float initFreq = 0.2f, float initAmp = 0.02f, float mergeT = 5f, 
                      float peakFreq = 0.6f, float peakAmp = 0.05f, float decayRate = 2f)
        {
            sourcePosition = sourcePos;
            destinationPosition = destPos;
            initialFrequency = initFreq;
            initialAmplitude = initAmp;
            mergeTime = mergeT;
            peakFrequency = peakFreq;
            peakAmplitude = peakAmp;
            postMergerDecayRate = decayRate;
        }

    }
}