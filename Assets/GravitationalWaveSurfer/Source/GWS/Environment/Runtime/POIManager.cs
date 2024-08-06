using UnityEngine;

using GWS.UI.Runtime;
using GWS.Quiz;
using GWS.Data;

namespace GWS.WorldGen
{
    /// <summary>
    /// Manages everything related to POIs: <br/>
    /// - checking for POIs in current chunk to show interact UI <br/>
    /// - generating UIs for each POI, randomly picking a question <br/>
    /// - turning on and off UIs
    /// </summary>
    public class POIManager : MonoBehaviour
    {
        public static POIManager Instance { get; private set; }

        [Header("Interaction settings")]
        public float interactionDistance = 100f;
        public KeyCode interactionKey = KeyCode.E;
        private bool interactionPermission = true;
        public bool interactionUIActive = false;
        public bool POIUIActive = false;

        [Space(6)]
        [Header("Relevant GameObjects")]
        public GameObject player;
        public Camera playerCamera;
        public GameObject currentPOI;

        [Space(6)]
        [Header("Quiz Questions")]
        public QuizQuestionDatabase questionDatabase;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); }    
        }

        void Start()
        {
            playerCamera = Camera.main;
            interactionDistance = ChunkManager.Instance.chunkSize / 3f;
        }

        void Update() 
        {
            if (interactionPermission)
            {
                CheckPOIInVicinity();
            }

            if (interactionUIActive && UnityEngine.Input.GetKeyDown(interactionKey) && !POIUIActive)
            {
                InteractWithPOI();
                POIUIActive = true;
                // restrict movements when in POI interaction menu?
            }
            else if (POIUIActive && UnityEngine.Input.GetKeyDown(interactionKey))
            {
                POI_UI.Instance.TogglePOIUI(false);
                POIUIActive = false;
            }
        }

        /// <summary>
        /// Handling cases when player shouldn't be able to interact with POIs <br/>
        /// i.e. when pause menu active
        /// </summary>
        /// <param name="value">true for interactable, false otherwise</param>
        public void ToggleInteractionPermission(bool value)
        {
            interactionPermission = value;
        }

        private void CheckPOIInVicinity()
        {
            Chunk currentChunk = ChunkManager.Instance.GetCurrentChunk();
            if (currentChunk.HasPOI)
            {
                // Debug.Log("chunk has POI");
                currentPOI = currentChunk.ChunkObject.transform.GetChild(0).gameObject;
                if (Vector3.Distance(player.transform.position, currentPOI.transform.position) < interactionDistance)
                {
                    if (!interactionUIActive) POI_UI.Instance.ToggleInteractionUI(true);
                    interactionUIActive = true;
                }
                else
                {
                    POI_UI.Instance.ToggleInteractionUI(false);
                    interactionUIActive = false;
                }
            }
            else
            {
                POI_UI.Instance.ToggleInteractionUI(false);
                interactionUIActive = false;
            }
        }
        
        private void InteractWithPOI()
        {
            if (currentPOI.CompareTag("POI"))
            {
                Debug.Log("Interacting with POI: " + currentPOI.name);
                POIData poiData = currentPOI.GetComponent<POIData>();
                if (poiData != null)
                {
                    QuizQuestion quizQuestion = questionDatabase.GetQuestionById(poiData.QuestionID);
                    POI_UI.Instance.TogglePOIUI(true, poiData.Available, poiData.Name, poiData.PassiveValue, poiData.OneTimeValue, quizQuestion);
                }
                else
                {
                    Debug.LogWarning("POIData of POI not found!!!");
                }
            }
            else if (currentPOI.CompareTag("Black Hole"))
            {
                Debug.Log("Interacting with Black Hole: " + currentPOI.name);
            }
        }

        /// <summary>
        /// Specifically for the exit button in POI UI
        /// </summary>
        public void DeactivatePOIUI()
        {
            POIUIActive = false;
            POI_UI.Instance.TogglePOIUI(false);
        }

        /// <summary>
        /// Helper function: toggles POIUIActive in POIManager
        /// </summary>
        /// <param name="value"></param>
        public void TogglePOIUIActive(bool value)
        {
            POIUIActive = value;
        }

        /// <summary>
        /// Changes whether if POI is available for quiz and rewards
        /// </summary>
        /// <param name="value"></param>
        public void ToggleCurrentPOIAvailability(bool value)
        {
            POIData poiData = currentPOI.GetComponent<POIData>();
            poiData.SetAvailability(value);
        }

    }
}