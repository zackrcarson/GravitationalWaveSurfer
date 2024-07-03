using GWS.GeneralRelativitySimulation.Runtime;
using UnityEngine;

namespace GWS.Timing.Runtime
{
    public class TimerButtonHandler : MonoBehaviour
    {
        [SerializeField]
        private TimeSpeedManager timeSpeedManager;
        private float increment = 1f;

        public void IncreaseScale() => TimeSpeedManager.Scale += increment;

        public void DecreaseScale() => TimeSpeedManager.Scale -= increment;
    }
}
