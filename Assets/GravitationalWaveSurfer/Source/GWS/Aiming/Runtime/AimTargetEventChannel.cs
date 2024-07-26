using System;
using UnityEngine;

namespace GWS.Aiming.Runtime
{
    /// <summary>
    /// Callbacks for setting aim target
    /// </summary>
    [CreateAssetMenu(fileName = nameof(AimTargetEventChannel), menuName = "ScriptableObjects/Aiming/Aim Target Event Channel")]
    public class AimTargetEventChannel : ScriptableObject
    {
        /// <summary>
        /// Callback on setting a lock on target.
        /// </summary>
        public event Action<Transform> OnSetLockOnTarget;
        
        /// <summary>
        /// Callback on unsetting a lock on target. 
        /// </summary>
        public event Action OnUnsetLockOnTarget;

        /// <summary>
        /// Raises the <see cref="OnSetLockOnTarget"/> event.
        /// </summary>
        /// <param name="position">The world-space <see cref="Transform"/> of the target to lock onto.</param>
        public void RaiseOnSetLockOnTarget(Transform position) => OnSetLockOnTarget?.Invoke(position);

        /// <summary>
        /// Raises the <see cref="OnUnsetLockOnTarget"/> event.
        /// </summary>
        public void RaiseOnUnsetLockOnTarget() => OnUnsetLockOnTarget?.Invoke(); 
    }
}