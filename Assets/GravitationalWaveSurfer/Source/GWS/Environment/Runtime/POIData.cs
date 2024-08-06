using UnityEngine;

namespace GWS.Data
{
    /// <summary>
    /// Helper class to store POI-specific data <br/>
    /// Needs to be attached on every POI Prefab
    /// </summary>
    public class POIData : MonoBehaviour
    {
        public string Name;
        public int PassiveValue { get; private set; }
        public int OneTimeValue { get; private set; }
        public bool Available { get; private set; }
        public int QuestionID { get; private set; }

        public void Initialize(int passiveValue, int oneTimeValue, int questionID)
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