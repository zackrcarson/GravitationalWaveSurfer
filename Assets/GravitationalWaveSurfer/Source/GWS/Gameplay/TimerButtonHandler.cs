using UnityEngine;
using GWS.GeneralRelativitySimulation.Runtime;

namespace GWS.Gameplay
{
    public class TimerButtonHandler : MonoBehaviour
    {
        [SerializeField]
        private TimeSpeedManager timeSpeedManager;
        private float increment = 1f;

        public void IncreaseScale() => timeSpeedManager.Scale += increment;

        public void DecreaseScale() => timeSpeedManager.Scale -= increment;
    }
}
