using UnityEngine;

namespace GWS.Data
{
    /// <summary>
    /// Helper class to store POI-specific data
    /// </summary>
    public class POIData : MonoBehaviour
    {
        public float PassiveValue { get; private set; }
        public float OneTimeValue { get; private set; }
        public bool Available { get; private set; } = true;

        public POIData(float passiveValue, float oneTimeValue)
        {
            PassiveValue = passiveValue;
            OneTimeValue = oneTimeValue;
        }

        public void SetAvailable(bool available)
        {
            Available = available;
        }
    }
}