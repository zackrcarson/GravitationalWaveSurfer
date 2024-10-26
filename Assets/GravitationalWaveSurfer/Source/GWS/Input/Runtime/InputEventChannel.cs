using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GWS.Input.Runtime
{
    /// <summary>
    /// Sends and receives event relating to input. 
    /// </summary>
    [CreateAssetMenu(fileName = "Input Event Channel", menuName = "ScriptableObjects/Input/Channels/Input Event Channel")]
    public class InputEventChannel: ScriptableObject
    {
        /// <summary>
        /// Callback on movement axis value changed. 
        /// </summary>
        public event Action<Vector2> OnMove;

        /// <summary>
        /// Callback on interact button pressed.  
        /// </summary>
        public event Action OnInteract;

        /// <summary>
        /// Callback on fire button pressed. 
        /// </summary>
        public event Action OnFire;
        
        /// <summary>
        /// Callback on fire2 button pressed. 
        /// </summary>
        public event Action<InputAction.CallbackContext> OnFire2;

        /// <summary>
        /// Callback on cursor position changed. 
        /// </summary>
        public event Action<Vector2> OnCursorPosition; 

        /// <summary>
        /// Callback on input activated. 
        /// </summary>
        public event Action OnActivateInput;

        /// <summary>
        /// Callback on input deactivated. 
        /// </summary>
        public event Action OnDeactivateInput;

        /// <summary>
        /// Raises the <see cref="OnMove"/> event. 
        /// </summary>
        /// <param name="value">The value of the movement.</param>
        public void RaiseOnMove(Vector2 value) => OnMove?.Invoke(value);

        /// <summary>
        /// Raises the <see cref="OnInteract"/> event. 
        /// </summary>
        public void RaiseOnInteract() => OnInteract?.Invoke();
        
        /// <summary>
        /// Raises the <see cref="OnFire"/> event. 
        /// </summary>
        public void RaiseOnFire() => OnFire?.Invoke();
        
        /// <summary>
        /// Raises the <see cref="OnFire2"/> event. 
        /// </summary>
        /// <param name="callbackContext">The callback context.</param>
        public void RaiseOnFire2(InputAction.CallbackContext callbackContext) => OnFire2?.Invoke(callbackContext);

        /// <summary>
        /// Raises the <see cref="OnCursorPosition"/> event.
        /// </summary>
        public void RaiseOnCursorPosition(Vector2 value) => OnCursorPosition?.Invoke(value);

        /// <summary>
        /// Raises the <see cref="OnActivateInput"/> event. 
        /// </summary>
        public void RaiseOnActivateInput() => OnActivateInput?.Invoke();

        /// <summary>
        /// Raises the <see cref="OnDeactivateInput"/> event. 
        /// </summary>
        public void RaiseOnDeactivateInput() => OnDeactivateInput?.Invoke(); 
    }
}