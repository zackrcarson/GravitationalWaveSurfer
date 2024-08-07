using UnityEngine;

namespace GWS.Data
{
    /// <summary>
    /// Helper class to store POI-specific data <br/>
    /// Needs to be attached on every POI Prefab <br/>
    /// Name field needs to be filled
    /// </summary>
    public class POIData : MonoBehaviour
    {
        [Header("Fill in the name field:")]
        public string Name;
        public double PassiveValue { get; private set; }
        public double OneTimeValue { get; private set; }
        public bool Available { get; private set; }
        public int QuestionID { get; private set; }

        public void Initialize(double passiveValue, double oneTimeValue, int questionID)
        {
            PassiveValue = passiveValue;
            OneTimeValue = oneTimeValue;
            QuestionID = questionID;
            Available = true;
        }

        public void SetAvailability(bool value)
        {
            Available = value;
        }
    }
}