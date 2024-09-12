using System.Threading;
using TMPro;
using UnityEngine;

namespace GWS.UIEffects.Runtime
{
    /// <summary>
    /// Effects for TextMeshPro components. 
    /// </summary>
    public static class TextMeshProEffects
    {
        /// <summary>
        /// Coroutine that gradually fades in the text of a <see cref="TextMeshProUGUI"/> element character by adjusting its vertices' alpha values.
        /// </summary>
        /// <param name="text">The TextMeshProUGUI element whose text will be faded in. </param>
        /// <param name="spread">Controls how many characters can be faded in at the same time. </param>
        /// <param name="speed">The speed at which each character fades in. </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        // modified from: https://discussions.unity.com/t/have-words-fade-in-one-by-one/697252/7
        public static async Awaitable FadeInTextAsync(TMP_Text text, float spread, float speed, CancellationToken cancellationToken)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            text.ForceMeshUpdate();

            var textInfo = text.textInfo;
            var currentCharacter = 0;
            var startingCharacterRange = currentCharacter;
            var characterCount = textInfo.characterCount;
            var fadeSteps = (byte)Mathf.Max(1, 255 / spread);

            while (startingCharacterRange < characterCount)
            {
                for (var i = startingCharacterRange; i <= currentCharacter; i++)
                {
                    if (!textInfo.characterInfo[i].isVisible) continue;

                    var materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                    var newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                    var vertexIndex = textInfo.characterInfo[i].vertexIndex;

                    // Update alpha for all 4 vertices
                    var alpha = (byte)Mathf.Clamp(newVertexColors[vertexIndex].a + fadeSteps, 0, 255);
                    for (var j = 0; j < 4; j++) newVertexColors[vertexIndex + j].a = alpha;
                    
                    if (alpha == 255) startingCharacterRange++;
                }

                text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                if (currentCharacter < characterCount - 1) currentCharacter++;
                await Awaitable.WaitForSecondsAsync(0.25f - speed * 0.01f, cancellationToken);
            }
        }
    }
}