using GWS.Input.Runtime;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GWS.Player.Runtime
{
    /// <summary>
    /// Toggles the aim camera
    /// </summary>
    public class PlayerToggleAimCamera: MonoBehaviour
    {
        [SerializeField] 
        private InputEventChannel inputEventChannel;

        [SerializeField] 
        private CinemachineVirtualCameraBase aimCamera;
        
        /// <summary>
        /// Camera to return to after aiming.
        /// </summary>
        /// I don't think there is a word for un-aiming
        [SerializeField] 
        private CinemachineVirtualCameraBase neutralCamera;
        
        [SerializeField]
        private int basePriority;

        private void OnEnable()
        {
            inputEventChannel.OnFire2 += HandleFire2;
        }

        private void OnDisable()
        {
            inputEventChannel.OnFire2 -= HandleFire2;
        }

        private void HandleFire2(InputAction.CallbackContext callbackContext)
        {
            var wasCancelled = callbackContext.canceled;
            neutralCamera.Priority = basePriority + (wasCancelled ? 1 : 0) ;
            aimCamera.Priority = basePriority + (wasCancelled ? 0 : 1);
        }
    }
}