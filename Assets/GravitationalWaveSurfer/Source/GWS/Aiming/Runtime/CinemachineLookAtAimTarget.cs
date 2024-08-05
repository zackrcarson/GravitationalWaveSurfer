using Unity.Cinemachine;
using UnityEngine;

namespace GWS.Aiming.Runtime
{
    
    public class CinemachineLookAtAimTarget: MonoBehaviour
    {
        [SerializeField] 
        private CinemachineVirtualCameraBase aimCamera;

        [SerializeField] 
        private AimData aimData;

        public void HandleOnCameraActivated(ICinemachineMixer _, ICinemachineCamera cinemachineCamera)
        {
            if (!ReferenceEquals(cinemachineCamera, aimCamera)) return; 
            aimCamera.LookAt = aimData.LockOnTarget;
        }
    }
}