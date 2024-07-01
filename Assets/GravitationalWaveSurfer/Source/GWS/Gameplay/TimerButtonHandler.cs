using UnityEngine;
using GWS.GeneralRelativitySimulation.Runtime;

namespace GWS.Gameplay
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
