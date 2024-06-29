using UnityEngine;
using TMPro;
using System;

namespace GWS.Gameplay
{
    /// <summary>
    /// Timer that player must complete phase one within.
    /// </summary>
    public class PhaseOneTimer: MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;

        private float elapsedTime = 120f;

        /// <summary>
        /// Emitted when timer finishes.
        /// </summary>
        public static event Action TimeUp;

        private bool done = false;

        void Update()
        {
            if (done) return;

            if (elapsedTime > 0) 
            {
                elapsedTime -= Time.deltaTime;
                elapsedTime = Mathf.Clamp(elapsedTime, 0, int.MaxValue);
            }

            if (elapsedTime == 0)
            {
                done = true;
                TimeUp?.Invoke();
            }

            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
