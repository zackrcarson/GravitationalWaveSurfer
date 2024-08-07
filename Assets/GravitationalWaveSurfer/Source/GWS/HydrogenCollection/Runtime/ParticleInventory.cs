using UnityEngine;

namespace GWS.HydrogenCollection.Runtime
{
    /// <summary>
    /// Tracks the count of collected particles.  
    /// </summary>
    [CreateAssetMenu(fileName = nameof(ParticleInventory), menuName = "ScriptableObjects/Hydrogen Collection/Particle Inventory")]
    public class ParticleInventory : ScriptableObject
    {
        [field: SerializeField]
        public double HydrogenCount { get; set; }
    }
}