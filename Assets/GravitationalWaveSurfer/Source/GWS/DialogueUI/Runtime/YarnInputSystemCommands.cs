using System.Collections;
using Mushakushi.YarnSpinnerUtility.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

namespace GWS.DialogueUI.Runtime
{
    public class YarnInputSystemCommands: MonoBehaviour
    {
        [SerializeField] private DialogueObserver dialogueObserver;
        [SerializeField] private InputActionAsset inputActionAsset;

        private void OnEnable()
        {
            // dialogueRunner.AddCommandHandler<string>("wait_for_input_action", WaitUntilInputActionPerformed);
            // dialogueRunner.AddCommandHandler<string>("enable_input_action", EnableInputAction);
            // dialogueRunner.AddCommandHandler<string>("disable_input_action", DisableInputAction);

            dialogueObserver.commandParsed.OnEvent += HandleCommandParsed;
        }

        private void OnDisable()
        {
            dialogueObserver.commandParsed.OnEvent -= HandleCommandParsed;
        }

        private void HandleCommandParsed(string[] commandElements)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Waits until an input action is performed. 
        /// </summary>
        /// <param name="actionNameOrID">Name of or path to the action</param>
        private IEnumerator WaitUntilInputActionPerformed(string actionNameOrID)
        {
            var action = inputActionAsset[actionNameOrID];
            if (action == null) yield break; 
            yield return new WaitUntil(() => action.WasPressedThisFrame() || action.WasPerformedThisFrame());
        }
        
        /// <summary>
        /// Enables an input action.
        /// </summary>
        /// <param name="actionNameOrID">Name of or path to the action</param>
        private void EnableInputAction(string actionNameOrID)
        {
            inputActionAsset[actionNameOrID]?.Enable();
        }
        
        /// <summary>
        /// Disables an input action. 
        /// </summary>
        /// <param name="actionNameOrID">Name of or path to the action</param>
        private void DisableInputAction(string actionNameOrID)
        {
            inputActionAsset[actionNameOrID]?.Disable();
        }
    }
}