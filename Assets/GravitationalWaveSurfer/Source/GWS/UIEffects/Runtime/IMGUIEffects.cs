using System.Threading;
using UnityEngine;

namespace GWS.UIEffects.Runtime
{
    public static class IMGUIEffects
    {
        /// <summary>
        /// Fades a canvas group <c>from</c> one t value <c>to</c> another over a certain <c>duration</c>.
        /// </summary>
        public static async Awaitable FadeAlpha(CanvasGroup canvasGroup, float from, float to, float duration, CancellationToken cancellationToken)
        {
            canvasGroup.alpha = from;

            var timeElapsed = 0f;
            while (!cancellationToken.IsCancellationRequested && timeElapsed < duration)
            {
                var t = timeElapsed / duration;
                timeElapsed += Time.deltaTime;

                var alpha = Mathf.Lerp(from, to, t);
                canvasGroup.alpha = alpha;

                await Awaitable.NextFrameAsync(cancellationToken);
            }

            canvasGroup.alpha = to;
            
            if (to == 0)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
    }
}