using System;
using UnityEngine;

namespace GWS.HydrogenCollection.Runtime
{
    /// <summary>
    /// Callbacks on <see cref="ParticleInventory"/> changes.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(ParticleInventoryEventChannel), menuName = "ScriptableObjects/Hydrogen Collection/Particle Inventory Event Channel")]
    public class ParticleInventoryEventChannel : ScriptableObject
    {
        /// <summary>
        /// Callback on hydrogen amount changed.
        /// </summary>
        public event Action<int> OnHydrogenCountChanged;

        /// <summary>
        /// Raises <see cref="OnHydrogenCountChanged"/>.
        /// </summary>
        /// <param name="newValue">The new hydrogen count.</param>
        public void RaiseOnHydrogenCountChanged(int newValue) => OnHydrogenCountChanged?.Invoke(newValue);
    }
}