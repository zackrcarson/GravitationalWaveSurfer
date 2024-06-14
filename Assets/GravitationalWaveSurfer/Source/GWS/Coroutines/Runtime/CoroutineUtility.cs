using System.Collections.Generic;
using MEC;
using UnityEngine;

namespace GWS.Coroutines.Runtime
{
    public static class CoroutineUtility
    {
        /// <summary>
        /// Invokes a callback in time seconds, then repeatedly every repeatRate seconds.
        /// </summary>
        /// <param name="callback">The callback to execute</param>
        /// <param name="time">The amount of time it takes to start invoking.</param>
        /// <param name="repeatRate">Repeats the callback at this rate in seconds.</param>
        /// <returns></returns>
        public static IEnumerator<float> InvokeRepeating(System.Action callback, float time, float repeatRate)
        {
            if (time > 0f) yield return Timing.WaitForSeconds(time); 
            
            while(true)
            {
                callback();
                yield return Timing.WaitForSeconds(repeatRate);
            }
            
            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// Fades a material in or out by modifying it's alpha.
        /// </summary>
        /// <param name="material">The material</param>
        /// <param name="seconds">The amount of seconds over which the fading will occur.</param>
        /// <param name="steps">The amount of times the alpha will be modified.</param>
        /// <param name="shouldFadeIn">Whether to fade in or out the material.</param>
        /// <returns></returns>
        public static IEnumerator<float> Fade(Material material, float seconds, float steps, bool shouldFadeIn)
        {
            var waitTime = seconds / steps;
            
            var color = material.color;
            color.a = shouldFadeIn ? 0 : 1;
            material.color = color;
            
            for (var i = 0f; i <= 1f; i += 1f / steps)
            {
                color = material.color;
                color.a = shouldFadeIn ? i : 1 - i;
                material.color = color;
                
                yield return Timing.WaitForSeconds(waitTime);
            }
            
            color = material.color;
            color.a = shouldFadeIn ? 1 : 0;
            material.color = color;
        }
    }
}