using System.Collections;
using Mushakushi.YarnSpinnerUtility.Runtime;
using UnityEngine;

namespace GWS.DialogueUI.Runtime
{
    public class DialogueAutoAdvance: MonoBehaviour
    {
        [SerializeField] private DialogueObserver dialogueObserver;
        [SerializeField] private float waitTime = 0.5f;

        private void OnEnable()
        {
            dialogueObserver.dialogueCompleted.OnEvent += HandleDialogueCompleted;
        }

        private void OnDisable()
        {
            dialogueObserver.dialogueCompleted.OnEvent -= HandleDialogueCompleted;
        }

        private void HandleDialogueCompleted() => StopAllCoroutines();

        private void Start()
        {
            dialogueObserver.nodeRequested.RaiseEvent("Start");
            dialogueObserver.dialogueStarted.RaiseEvent();
            StartCoroutine(_Continue());
        }

        private IEnumerator _Continue()
        {
            while (true)
            {
                dialogueObserver.continueRequested.RaiseEvent();
                yield return new WaitForSeconds(waitTime);
            }
            
            // ReSharper disable once IteratorNeverReturns
        }
    }
}