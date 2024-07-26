using Unity.Cinemachine;
using UnityEngine;

namespace GWS.Aiming.Runtime
{
    /// <summary>
    /// Attach to <see cref="CinemachineVirtualCameraBase"/> to hard look at aim target.
    /// </summary>
    public class CinemachineHardLookAtAimTarget: MonoBehaviour
    {
        [SerializeField] 
        private AimTargetEventChannel aimTargetEventChannel;
        
        [SerializeField] 
        private CinemachineVirtualCameraBase virtualCamera;

        private void OnEnable()
        {
            aimTargetEventChannel.OnSetLockOnTarget += HandleSetLockOnTarget;
            aimTargetEventChannel.OnUnsetLockOnTarget += HandleUnsetLockOnTarget;
        }

        private void HandleSetLockOnTarget(Transform target)
        {
            virtualCamera.LookAt = target;
        }

        private void HandleUnsetLockOnTarget()
        {
            virtualCamera.LookAt = null;
        }
    }
}