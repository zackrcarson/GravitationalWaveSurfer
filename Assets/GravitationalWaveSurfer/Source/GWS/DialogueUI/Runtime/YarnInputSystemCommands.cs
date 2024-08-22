using System.Collections;
using Mushakushi.YarnSpinnerUtility.Runtime;
using Mushakushi.YarnSpinnerUtility.Runtime.Commands;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GWS.DialogueUI.Runtime
{
    public class YarnInputSystemCommands: MonoBehaviour
    {
        [SerializeField] private YarnCommandController yarnCommandController;
        [SerializeField] private DialogueObserver dialogueObserver;
        [SerializeField] private InputActionAsset inputActionAsset;

        private void Awake()
        {
            yarnCommandController.AddCommandHandler<string>("wait_for_input_action", WaitUntilInputActionPerformed);
            yarnCommandController.AddCommandHandler<string>("enable_input_action", EnableInputAction);
            yarnCommandController.AddCommandHandler<string>("disable_input_action", DisableInputAction);
        }

        /// <summary>
        /// Waits until an input action is performed. 
        /// </summary>
        /// <param name="actionNameOrID">Name of or path to the action</param>
        private void WaitUntilInputActionPerformed(string actionNameOrID) => StartCoroutine(_WaitUntilInputActionPerformed(actionNameOrID));
        
        private IEnumerator _WaitUntilInputActionPerformed(string actionNameOrID)
        {
            var action = inputActionAsset[actionNameOrID];
            if (action == null) yield break; 
            yield return new WaitUntil(() => action.WasPressedThisFrame() || action.WasPerformedThisFrame());
            
            dialogueObserver.commandHandled.RaiseEvent();
        }
        
        /// <summary>
        /// Enables an input action.
        /// </summary>
        /// <param name="actionNameOrID">Name of or path to the action</param>
        private void EnableInputAction(string actionNameOrID)
        {
            inputActionAsset[actionNameOrID]?.Enable();
            dialogueObserver.commandHandled.RaiseEvent();
        }
        
        /// <summary>
        /// Disables an input action. 
        /// </summary>
        /// <param name="actionNameOrID">Name of or path to the action</param>
        private void DisableInputAction(string actionNameOrID)
        {
            inputActionAsset[actionNameOrID]?.Disable();
            dialogueObserver.commandHandled.RaiseEvent();
        }
    }
}