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
        private float fixedDeltaTime;

        private float scale = 1f;

        public float Scale
        {
            get => scale;
            set => scale = Mathf.Clamp(value, 0f, 100f); 
        }

        private void Awake()
        {
            fixedDeltaTime = Time.fixedDeltaTime;
        }

        private void Update()
        {
            Time.timeScale = scale;
            Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
        }
    }
}
