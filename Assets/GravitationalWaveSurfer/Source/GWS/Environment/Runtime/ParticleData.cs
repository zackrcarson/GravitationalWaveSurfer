using UnityEngine;

namespace GWS.Data
{
    /// <summary>
    /// Helper class to store particle-specific data
    /// </summary>
    public class ParticleData : MonoBehaviour
    {
        public Vector3 initialPosition;
        public Vector3 projectedPoint;
        public float particleElapsedTime;
        public float propagationDelay;
        public float amplitudeScale;

        public ParticleData(Vector3 position)
        {
            initialPosition = position;
        }
    }
}