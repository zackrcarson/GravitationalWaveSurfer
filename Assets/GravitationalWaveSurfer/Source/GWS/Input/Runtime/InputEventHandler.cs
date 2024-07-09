using UnityEngine;
using UnityEngine.InputSystem;

namespace GWS.Input.Runtime
{
    /// <summary>
    /// Broadcasts input events. 
    /// </summary>
    public class InputEventHandler : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="PlayerInput"/>.
        /// </summary>
        [SerializeField, Tooltip("The Player Input Component.")] 
        private PlayerInput playerInput;

        /// <summary>
        /// The <see cref="InputEventChannel"/> to send input events to. 
        /// </summary>
        [SerializeField, Tooltip("The InputEventChannel to send input events to.")]
        private InputEventChannel inputEventChannel; 
    
        public void OnMove(InputAction.CallbackContext callbackContext)
        {
            inputEventChannel.RaiseOnMove(callbackContext.ReadValue<Vector2>());
        }

        public void OnFire(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.started) return;
            inputEventChannel.RaiseOnFire();
        }
        
        public void OnFire2(InputAction.CallbackContext callbackContext)
        {
            inputEventChannel.RaiseOnFire2(callbackContext);
        }

        public void OnCursorPosition(InputAction.CallbackContext callbackContext)
        {
            inputEventChannel.RaiseOnCursorPosition(callbackContext.ReadValue<Vector2>()); 
        }

        public void OnInteract(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.started) return;
            inputEventChannel.RaiseOnInteract();
        }

        public void DeactivateInput()
        {
            playerInput.DeactivateInput();
        }

        public void ActivateInput()
        {
            playerInput.ActivateInput();
        }
    }
}
