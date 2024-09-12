using YarnSpinnerUtility.Runtime.Commands;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GWS.DialogueUI.Runtime
{
    public class YarnInputSystemCommands: MonoBehaviour
    {
        [SerializeField] private YarnCommandDispatcher yarnCommandDispatcher;
        [SerializeField] private InputActionAsset inputActionAsset;

        private void Awake()
        {
            yarnCommandDispatcher.AddCommandHandler<string>("wait_for_input_action", WaitUntilInputActionPerformed);
            yarnCommandDispatcher.AddCommandHandler<string>("enable_input_action", EnableInputAction);
            yarnCommandDispatcher.AddCommandHandler<string>("disable_input_action", DisableInputAction);
        }

        /// <summary>
        /// Waits until an input action is performed. 
        /// </summary>
        /// <param name="actionNameOrID">Name of or path to the action</param>
        private async void WaitUntilInputActionPerformed(string actionNameOrID)
        {
            var action = inputActionAsset[actionNameOrID];
            if (action == null)
            {
                yarnCommandDispatcher.dialogueParser.TryContinue();
                return;
            }

            while (!action.WasPerformedThisFrame() && !action.WasPressedThisFrame()) await Awaitable.NextFrameAsync();
            yarnCommandDispatcher.dialogueParser.TryContinue();
        }
        
        /// <summary>
        /// Enables an input action.
        /// </summary>
        /// <param name="actionNameOrID">Name of or path to the action</param>
        private void EnableInputAction(string actionNameOrID)
        {
            inputActionAsset[actionNameOrID]?.Enable();
            yarnCommandDispatcher.dialogueParser.TryContinue();
        }
        
        /// <summary>
        /// Disables an input action. 
        /// </summary>
        /// <param name="actionNameOrID">Name of or path to the action</param>
        private void DisableInputAction(string actionNameOrID)
        {
            inputActionAsset[actionNameOrID]?.Disable();
            yarnCommandDispatcher.dialogueParser.TryContinue();
        }
    }
}