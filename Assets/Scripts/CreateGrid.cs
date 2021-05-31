using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CreateGrid : MonoBehaviour
{
    // Config Parameters
    [SerializeField] public int gridWidth = 100;
    [SerializeField] int gridHeight = 50;
    [SerializeField] float gridMarkerScale = 0.01f;

    [SerializeField] GameObject gridMarker = null;
    [SerializeField] GameObject gridMarkerParent = null;
    [SerializeField] GameObject mainCanvas = null;

    // Cached References
    float cameraHeight;
    float cameraWidth;

    void Start()
    {
        if (gridHeight % 2 == 0)
        {
            gridHeight = gridHeight + 1;
        }

        DestroyGrid();
        MakeGrid();
    }

    private void MakeGrid()
    {
        cameraHeight = Camera.main.orthographicSize;
        cameraWidth = cameraHeight * mainCanvas.GetComponent<CanvasScaler>().referenceResolution.x / mainCanvas.GetComponent<CanvasScaler>().referenceResolution.y;//1920 / 1080;

        for (int x = 0; x <= gridWidth - 1; x++)
        {
            GameObject currentSlice = new GameObject("slice_" + x);
            currentSlice.transform.position = new Vector3(-cameraWidth + 2 * x * cameraWidth / (gridWidth - 1), 0, 0);
            currentSlice.transform.parent = gridMarkerParent.transform;

            for (int y = 0; y <= gridHeight - 1; y++)
            {
                GameObject go = Instantiate(gridMarker);

                float yMarkerPosition = -cameraHeight + 2 * y * cameraHeight / (gridHeight - 1);
                float xMarkerPosition = -cameraWidth + 2 * x * cameraWidth / (gridWidth - 1);

                go.transform.position = new Vector3(xMarkerPosition, yMarkerPosition, 0);
                go.transform.localScale = new Vector3(gridMarkerScale, gridMarkerScale, 0);

                go.transform.parent = currentSlice.transform;
                go.transform.name = "marker_" + y;
            }
        }
    }

    void DestroyGrid()
    {
        GameObject[] objsToDestroy = new GameObject[gridMarkerParent.transform.childCount];
        int idx = 0;
        foreach (Transform child in gridMarkerParent.transform)
        {
            objsToDestroy[idx++] = child.gameObject;
        }

        foreach (GameObject obj in objsToDestroy)
        {
            DestroyImmediate(obj);
        }
    }
}