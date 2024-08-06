using UnityEngine;
using UnityEngine.UI;
using TMPro;

using GWS.Quiz;
using GWS.HydrogenCollection.Runtime;
using GWS.GameStage.Runtime;

namespace GWS.WorldGen
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
        public GameObject POIWrongAnswerObject;
        public GameObject POIUnavailableObject;
        
        private void Awake() {
            if (Instance == null)
            {
                Instance = this;
                // DontDestroyOnLoad(gameObject);
            }
            else { Destroy(gameObject); }

            if (interactionUIObject == null) { Debug.LogWarning("No interaction UI object set!"); }
            if (POIUIObject == null) { Debug.LogWarning("No POI UI object set!"); }
        }

        private void Start()
        {
            POIPromptObject = POIUIObject.transform.Find("POIPrompt").gameObject;
            POIQuestionObject = POIUIObject.transform.Find("POIQuestion").gameObject;
            POIRewardsObject = POIUIObject.transform.Find("POIRewards").gameObject;
            POIWrongAnswerObject = POIUIObject.transform.Find("POIWrongAnswer").gameObject;
            POIUnavailableObject = POIUIObject.transform.Find("POIUnavailable").gameObject;

            if (POIPromptObject == null || POIQuestionObject == null || 
                POIRewardsObject == null || POIWrongAnswerObject == null ||
                POIUnavailableObject == null)
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

        /// <summary>
        /// Toggles the indicative UI of when POI is interactable
        /// </summary>
        /// <param name="value"></param>
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
        /// <param name="question"></param>
        public void TogglePOIUI(bool value, bool availability=false, string name="POI", int passiveValue=0, int oneTimeValue=0, QuizQuestion question=null)
        {
            if (!value)
            {
                POIPromptObject.gameObject.SetActive(false);
                POIQuestionObject.gameObject.SetActive(false);
                POIRewardsObject.gameObject.SetActive(false);    
                POIWrongAnswerObject.gameObject.SetActive(false);       
                POIUnavailableObject.gameObject.SetActive(false); 
            }
            else
            {
                // if POI not available, show unavailable UI
                if (!availability)
                {
                    POIUnavailableObject.gameObject.SetActive(true);

                    Button hmmButton = POIUnavailableObject.transform.Find("Panel/HmmButton").GetComponent<Button>();
                    hmmButton.onClick.RemoveAllListeners();
                    hmmButton.onClick.AddListener(() => TogglePOIUI(false));
                    hmmButton.onClick.AddListener(() => POIManager.Instance.TogglePOIUIActive(false));
                    return;
                }

                POIPromptObject.gameObject.SetActive(true);

                // --------------set up prompt--------------
                TextMeshProUGUI nameText = POIPromptObject.transform.Find("Panel/Name").GetComponent<TextMeshProUGUI>();
                nameText.text = name;

                Button continueButton = POIPromptObject.transform.Find("Panel/ContinueButton").GetComponent<Button>();
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(() => ToggleNextUI(POIPromptObject, POIQuestionObject));

                // --------------set up question--------------
                if (question != null)
                {
                    TextMeshProUGUI questionText = POIQuestionObject.transform.Find("Panel/Question").GetComponent<TextMeshProUGUI>();
                    questionText.text = question.questionText;

                    for (int i = 0; i < question.answerOptions.Length; i++)
                    {
                        Button answerButton = POIQuestionObject.transform.Find($"Panel/Option{i+1}").GetComponent<Button>();
                        TextMeshProUGUI answerText = answerButton.GetComponentInChildren<TextMeshProUGUI>();
                        answerText.text = question.answerOptions[i];

                        int index = i;  // new variable because passing i will be a reference so i ends up being 4 rather than [0, 3]
                        answerButton.onClick.RemoveAllListeners();
                        answerButton.onClick.AddListener(() => OnAnswerSelected(index, question.correctAnswerIndex));
                    }
                }
                else
                {
                    Debug.LogWarning("POI Question is null!!!");
                }

                // -------------- set up rewards--------------
                Button oneTimeButton = POIRewardsObject.transform.Find("Panel/OneTimeButton").GetComponent<Button>();
                Button passiveButton = POIRewardsObject.transform.Find("Panel/PassiveButton").GetComponent<Button>();
                TextMeshProUGUI oneTimeText = oneTimeButton.GetComponentInChildren<TextMeshProUGUI>();
                TextMeshProUGUI passiveText = passiveButton.GetComponentInChildren<TextMeshProUGUI>();

                // change texts for this POI
                oneTimeText.text = $"+{oneTimeValue} Mass";
                passiveText.text = $"+{passiveValue} Mass/sec";

                oneTimeButton.onClick.RemoveAllListeners();
                passiveButton.onClick.RemoveAllListeners();

                oneTimeButton.onClick.AddListener(() => HydrogenManager.Instance.AddHydrogen(oneTimeValue));
                oneTimeButton.onClick.AddListener(() => TogglePOIUI(false));
                oneTimeButton.onClick.AddListener(() => POIManager.Instance.TogglePOIUIActive(false));
                oneTimeButton.onClick.AddListener(() => POIManager.Instance.ToggleCurrentPOIAvailability(false));
                oneTimeButton.onClick.AddListener(() => GameStageManager.Instance.GameStageIncQuiz());

                passiveButton.onClick.AddListener(() => HydrogenPassiveCollection.Instance.ChangePassiveCollection(passiveValue));
                passiveButton.onClick.AddListener(() => TogglePOIUI(false));
                passiveButton.onClick.AddListener(() => POIManager.Instance.TogglePOIUIActive(false));
                passiveButton.onClick.AddListener(() => POIManager.Instance.ToggleCurrentPOIAvailability(false));
                passiveButton.onClick.AddListener(() => GameStageManager.Instance.GameStageIncQuiz());

                TextMeshProUGUI multiplierIncText = POIRewardsObject.transform.Find("Panel/MultiplierInc").GetComponent<TextMeshProUGUI>();
                multiplierIncText.text = $"(multiplier * 10^{GameStageManager.Instance.incPerQuizQuestion})";

                // --------------set up wrong answer texts and button--------------
                TextMeshProUGUI answertext = POIWrongAnswerObject.transform.Find("Panel/Answer").GetComponent<TextMeshProUGUI>();
                answertext.text = question.answerOptions[question.correctAnswerIndex];

                Button nowIKnowButton = POIWrongAnswerObject.transform.Find("Panel/NowIKnowButton").GetComponent<Button>();
                nowIKnowButton.onClick.RemoveAllListeners();
                nowIKnowButton.onClick.AddListener(() => TogglePOIUI(false));
                nowIKnowButton.onClick.AddListener(() => POIManager.Instance.TogglePOIUIActive(false));
                nowIKnowButton.onClick.AddListener(() => POIManager.Instance.ToggleCurrentPOIAvailability(false));
            }
        }

        /// <summary>
        /// Change to a different UI
        /// </summary>
        /// <param name="PrevUI"></param>
        /// <param name="NextUI"></param>
        private void ToggleNextUI(GameObject PrevUI, GameObject NextUI)
        {
            PrevUI.gameObject.SetActive(false);
            NextUI.gameObject.SetActive(true);
        }

        /// <summary>
        /// Checks the answer for the quiz question and proceeds accordingly
        /// </summary>
        /// <param name="answerIndex"></param>
        /// <param name="correctIndex"></param>
        private void OnAnswerSelected(int answerIndex, int correctIndex)
        {
            bool isCorrect = answerIndex == correctIndex;

            if (isCorrect)
            {
                ToggleNextUI(POIQuestionObject, POIRewardsObject);
            }
            else
            {
                ToggleNextUI(POIQuestionObject, POIWrongAnswerObject);

                // if player exits POI UI sequence at this point, POI should still be set as unavailable 
                POIManager.Instance.ToggleCurrentPOIAvailability(false);    
            }    
        }
    }
}