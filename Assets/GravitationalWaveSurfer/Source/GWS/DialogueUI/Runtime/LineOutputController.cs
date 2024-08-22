using Cysharp.Threading.Tasks;
using Mushakushi.YarnSpinnerUtility.Runtime;
using Mushakushi.YarnSpinnerUtility.Runtime.Output;
using UnityEngine;
using Yarn.Unity;

namespace GWS.DialogueUI.Runtime
{
    /// <summary>
    /// Outputs dialogue body and character name output from a <see cref="LocalizedLine"/>.
    /// </summary>
    public class LineOutputController: MonoBehaviour
    {
        [SerializeField] private DialogueObserver dialogueObserver;
        [SerializeField] private CanvasGroup dialogueCanvasGroup;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeReference, SubclassSelector] private ITextOutput dialogueBodyOutput;
        
        private void OnEnable()
        {
            dialogueObserver.dialogueStarted.OnEvent += HandleDialogueStarted;
            dialogueObserver.lineParsed.OnEvent += OutputLine;
            dialogueObserver.dialogueCompleted.OnEvent += HandleDialogueCompleted;
        }

        private void OnDisable()
        {
            dialogueObserver.dialogueStarted.OnEvent -= HandleDialogueStarted;
            dialogueObserver.lineParsed.OnEvent -= OutputLine;
            dialogueObserver.dialogueCompleted.OnEvent -= HandleDialogueCompleted;
        }
        
        private void HandleDialogueStarted()
        {
            StartCoroutine(Effects.FadeAlpha(dialogueCanvasGroup, 0, 1, fadeDuration));
        }

        private void HandleDialogueCompleted()
        {
            StartCoroutine(Effects.FadeAlpha(dialogueCanvasGroup, 1, 0, fadeDuration));
        }
        
        private void OutputLine(LocalizedLine localizedLine)
        {
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => Mathf.Approximately(dialogueCanvasGroup.alpha, 1));
                dialogueBodyOutput.Write(localizedLine.TextWithoutCharacterName.Text);
            });
        }
    }
}