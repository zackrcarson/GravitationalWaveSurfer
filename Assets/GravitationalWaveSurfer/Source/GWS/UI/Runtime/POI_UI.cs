using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace GWS.UI.Runtime
{
    public class POI_UI : MonoBehaviour
    {
        public static POI_UI Instance { get; private set; }

        public GameObject interactionUIPrefab;
        // public GameObject POIUIPrefab;
        // public bool interactionUIActive = false;
        // public bool POIUIActive = false;
        
        private void Awake() {
            if (Instance == null)
            {
                Instance = this;
                // DontDestroyOnLoad(gameObject);
            }
            else { Destroy(gameObject); }

            if (interactionUIPrefab == null) { Debug.LogWarning("No interaction UI prefab set!"); }
            // if (POIUIPrefab == null) { Debug.LogWarning("No POI UI prefab set!"); }

            interactionUIPrefab.gameObject.SetActive(false);
        }

        public void ToggleInteractionUI(bool value)
        {
            interactionUIPrefab.gameObject.SetActive(value);
        }

    }
}
