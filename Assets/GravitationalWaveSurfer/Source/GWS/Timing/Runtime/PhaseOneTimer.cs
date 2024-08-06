using System;
using TMPro;
using UnityEngine;

namespace GWS.Timing.Runtime
{
    /// <summary>
    /// Timer that player must complete phase one within.
    /// </summary>
    public class PhaseOneTimer: MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;

        public static PhaseOneTimer Instance { get; private set; }
        public float phaseOneTotalTime = 600f;
        private float elapsedTime = 600f;

        /// <summary>
        /// Emitted when timer finishes.
        /// </summary>
        public static event Action TimeUp;

        private bool done = false;

        private void Awake()
        {
            if (Instance = null) Instance = this;    
        }

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
            text.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
