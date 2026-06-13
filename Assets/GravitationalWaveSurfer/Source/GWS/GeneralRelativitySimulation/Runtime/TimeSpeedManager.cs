using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GWS.GeneralRelativitySimulation.Runtime
{
    /// <summary>
    /// Very temporary implementation of time scaling.
    /// </summary>
    public class TimeSpeedManager : MonoBehaviour
    {
        private static float fixedDeltaTime;

        private static float scale = 1f;

        public static float Scale
        {
            get => scale;
            set => scale = Mathf.Clamp(value, 0f, 100f); 
        }

        private void Awake()
        {
            fixedDeltaTime = Time.fixedDeltaTime;
            Scale = 1f;
        }

        private float appliedScale = -1f;

        private void Update()
        {
            if (Mathf.Approximately(scale, appliedScale)) return;
            appliedScale = scale;
            Time.timeScale = scale;
            Time.fixedDeltaTime = fixedDeltaTime * scale;
        }
    }
}
