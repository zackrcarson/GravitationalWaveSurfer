using UnityEngine;
using UnityEngine.Serialization;

namespace GWS.Aiming.Runtime
{
    /// <summary>
    /// Stores data on the current aim position.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(AimData), menuName = "ScriptableObjects/Aiming/Aim Data")]
    public class AimData : ScriptableObject
    {
        /// <summary>
        /// The screen-space position of the aim, with the z axis representing depth.
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// The target to be locked onto. 
        /// </summary>
        public Transform LockOnTarget { get; set; } 

        /// <summary>
        /// The camera used for aim calculations.
        /// </summary>
        public Camera camera;
    }
}