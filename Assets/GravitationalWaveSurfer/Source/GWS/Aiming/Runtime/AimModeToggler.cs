using UnityEngine;

namespace GWS.Aiming.Runtime
{
    /// <summary>
    /// Toggles the aim mode based on <see cref="aimTargetEventChannel"/> events.
    /// </summary>
    public class AimModeToggler: MonoBehaviour
    {
        [SerializeField] 
        private AimTargetEventChannel aimTargetEventChannel;
        
        [SerializeField]
        private AimFollowCursor aimFollowCursor;
        
        [SerializeField]
        private AimFollowTarget aimFollowTarget;

        private void OnEnable()
        {
            aimTargetEventChannel.OnSetLockOnTarget += HandleSetLockOnTarget;
            aimTargetEventChannel.OnUnsetLockOnTarget += HandleUnsetLockOnTarget;
        }

        private void OnDisable()
        {
            aimTargetEventChannel.OnSetLockOnTarget -= HandleSetLockOnTarget;
            aimTargetEventChannel.OnUnsetLockOnTarget -= HandleUnsetLockOnTarget;
        }

        private void HandleSetLockOnTarget(Transform target)
        {
            aimFollowCursor.enabled = false;
            
            aimFollowTarget.enabled = true;
            aimFollowTarget.aimTarget = target;
        }
        
        private void HandleUnsetLockOnTarget()
        {
            aimFollowTarget.enabled = false;
            aimFollowCursor.enabled = true;
        }

        private void Start() => HandleUnsetLockOnTarget();
    }
}