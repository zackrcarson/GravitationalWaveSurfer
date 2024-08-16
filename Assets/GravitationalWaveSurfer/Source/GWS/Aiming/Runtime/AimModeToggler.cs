using GWS.Input.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GWS.Aiming.Runtime
{
    /// <summary>
    /// Toggles the aim mode based on <see cref="inputEventChannel"/> events.
    /// </summary>
    public class AimModeToggler: MonoBehaviour
    {
        [SerializeField] 
        private InputEventChannel inputEventChannel;

        [SerializeField] 
        private AimData aimData;
        
        [SerializeField]
        private AimModeFollowCursor aimModeFollowCursor;
        
        [SerializeField]
        private AimModeFollowTransform aimModeFollowTransform;

        private void OnEnable()
        {
            inputEventChannel.OnFire2 += HandleSetLockOnTarget;
        }

        private void OnDisable()
        {
            inputEventChannel.OnFire2 -= HandleSetLockOnTarget;
        }
        
        private void Start() => HandleUnsetLockOnTarget();

        private void HandleSetLockOnTarget(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.canceled)
            {
                HandleUnsetLockOnTarget();
            }
            else if (aimData.LockOnTarget)
            {
                HandleSetLockOnTarget();
            }
        }

        private void HandleSetLockOnTarget()
        {
            aimModeFollowCursor.enabled = false;
            aimModeFollowTransform.enabled = true;
        }
        
        private void HandleUnsetLockOnTarget()
        {
            aimModeFollowCursor.enabled = true;
            aimModeFollowTransform.enabled = false;
        }
    }
}