using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Codice.Client.BaseCommands.Merge.Xml;

namespace GWS.UI.Runtime
{
    /// <summary>
    /// Works with POIManager.cs for toggling UIs related to POIs
    /// </summary>
    public class POI_UI : MonoBehaviour
    {
        public static POI_UI Instance { get; private set; }

        /// <summary>
        /// UI showing the option to interact with the POI
        /// </summary>
        public GameObject interactionUIObject;
        /// <summary>
        /// UI after player interacts with the POI
        /// </summary>
        public GameObject POIUIObject;
        public GameObject POIPromptObject;
        public GameObject POIQuestionObject;
        public GameObject POIRewardsObject;

        // public bool interactionUIActive = false;
        // public bool POIUIActive = false;
        
        private void Awake() {
            if (Instance == null)
            {
                Instance = this;
                // DontDestroyOnLoad(gameObject);
            }
            else { Destroy(gameObject); }

            if (interactionUIObject == null) { Debug.LogWarning("No interaction UI prefab set!"); }
            // if (POIUIObject == null) { Debug.LogWarning("No POI UI prefab set!"); }

        }

        private void Start()
        {
            POIPromptObject = POIUIObject.transform.Find("POIPrompt").gameObject;
            POIQuestionObject = POIUIObject.transform.Find("POIQuestion").gameObject;
            POIRewardsObject = POIUIObject.transform.Find("POIRewards").gameObject;

            if (POIPromptObject == null || POIQuestionObject == null || POIRewardsObject == null)
            {
                Debug.LogWarning("POI UI object(s) missing!!!");
            }

            InitializeUIObjects();
        }

        /// <summary>
        /// Makes sure all POI UIs are inactive in the beginning <br/>
        /// Called by Start() of POI_UI
        /// </summary>
        public void InitializeUIObjects()
        {
            interactionUIObject.gameObject.SetActive(false);
            POIPromptObject.gameObject.SetActive(false);
            POIQuestionObject.gameObject.SetActive(false);
            POIRewardsObject.gameObject.SetActive(false);
        }

        public void ToggleInteractionUI(bool value)
        {
            interactionUIObject.gameObject.SetActive(value);
        }

        /// <summary>
        /// In charge of starting POI UI sequence & closing it <br/>
        /// Sets up a random question chosen from a list from POIManager.cs <br/>
        /// Sets up the correct values for rewards for a POI <br/>
        /// </summary>
        /// <param name="value">true for showing UI, false for closing it</param>
        /// <param name="passiveValue"></param>
        /// <param name="oneTimeValue"></param>
        public void TogglePOIUI(bool value, float passiveValue=0, float oneTimeValue=0)
        {
            if (!value)
            {
                POIPromptObject.gameObject.SetActive(false);
                POIQuestionObject.gameObject.SetActive(false);
                POIRewardsObject.gameObject.SetActive(false);            
            }
            else
            {
                POIPromptObject.gameObject.SetActive(true);

                // set up question
                

                // set up rewards
                TextMeshProUGUI oneTimeText = POIRewardsObject.transform.Find("Panel/OneTimeButton/Text").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI passiveText = POIRewardsObject.transform.Find("Panel/PassiveButton/Text").GetComponent<TextMeshProUGUI>();

                oneTimeText.text = $"+{oneTimeValue} Mass";
                passiveText.text = $"+{passiveValue} Mass/sec";
            }
        }

    }
}
