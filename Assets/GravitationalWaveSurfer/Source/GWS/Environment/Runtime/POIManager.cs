using UnityEngine;
using UnityEngine.UI;

using GWS.UI.Runtime;
using GWS.WorldGen;

public class POIManager : MonoBehaviour
{
    [Header("Interaction settings")]
    public float interactionDistance = 100f;
    public KeyCode interactionKey = KeyCode.E;
    private bool interactionPermission = true;
    public bool interactionUIActive = false;

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

        if (interactionUIActive && Input.GetKeyDown(interactionKey))
        {
            InteractWithPOI();
        }
    }

    public void toggleInteractionPermission(bool value)
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
        }
        else if (currentPOI.CompareTag("Black Hole"))
        {
            Debug.Log("Interacting with Black Hole: " + currentPOI.name);
        }
    }

}