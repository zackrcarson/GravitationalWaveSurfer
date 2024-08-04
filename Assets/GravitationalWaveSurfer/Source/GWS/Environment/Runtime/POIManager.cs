using UnityEngine;
using UnityEngine.UI;

using GWS.UI.Runtime;
using GWS.WorldGen;

/// <summary>
/// Manages everything related to POIs: <br/>
/// - checking for POIs in current chunk to show interact UI <br/>
/// - generating UIs for each POI, randomly picking a question <br/>
/// - turning on and off UIs
/// </summary>
public class POIManager : MonoBehaviour
{
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

    void Start()
    {
        playerCamera = Camera.main;
        interactionDistance = ChunkManager.Instance.chunkSize / 4f;
    }

    void Update() 
    {
        if (interactionPermission)
        {
            CheckPOIInVicinity();
        }

        if (interactionUIActive && Input.GetKeyDown(interactionKey) && !POIUIActive)
        {
            InteractWithPOI();
            POIUIActive = true;
            // restrict movements when in POI interaction menu?
        }
        else if (POIUIActive && Input.GetKeyDown(interactionKey))
        {
            POI_UI.Instance.TogglePOIUI(false);
            POIUIActive = false;
        }
    }

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
                if (!interactionUIActive) ShowPOIPrompt();
                interactionUIActive = true;
            }
            else
            {
                HidePOIPrompt();
                interactionUIActive = false;
            }
        }
        else
        {
            HidePOIPrompt();
            interactionUIActive = false;
        }
    }

    private void ShowPOIPrompt()
    {
        POI_UI.Instance.ToggleInteractionUI(true);
    }

    private void HidePOIPrompt()
    {
        POI_UI.Instance.ToggleInteractionUI(false);
    }
    
    private void InteractWithPOI()
    {
        if (currentPOI.CompareTag("POI"))
        {
            Debug.Log("Interacting with POI: " + currentPOI.name);
            POI_UI.Instance.TogglePOIUI(true, 10, 1000);
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

}