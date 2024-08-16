using System;
using UnityEngine;

namespace GWS.CollisionEvents.Runtime
{
    /// <summary>
    /// Raises events associated with collision.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(CollisionEventChannel), menuName = "ScriptableObjects/Collision Events/Collision Event Channel")]
    public class CollisionEventChannel : ScriptableObject
    {
        /// <summary>
        /// Callback on collision enter.
        /// </summary>
        public event Action<Collider> OnTriggerEnter;

        /// <summary>
        /// Callback on collision stay.
        /// </summary>
        public event Action<Collider> OnTriggerStay;

        /// <summary>
        /// Callback on collision exit.
        /// </summary>
        public event Action<Collider> OnTriggerExit;

        /// <summary>
        /// Raises the <see cref="OnTriggerEnter"/> event.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> associated with the collision.</param>
        public void RaiseOnTriggerEnter(Collider other) => OnTriggerEnter?.Invoke(other);
        
        /// <summary>
        /// Raises the <see cref="OnTriggerStay"/> event.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> associated with the collision.</param>
        public void RaiseOnTriggerStay(Collider other) => OnTriggerStay?.Invoke(other);
        
        /// <summary>
        /// Raises the <see cref="OnTriggerExit"/> event.
        /// </summary>
        /// <param name="other">The other <see cref="Collider"/> associated with the collision.</param>
        public void RaiseOnTriggerExit(Collider other) => OnTriggerExit?.Invoke(other);
    }
}