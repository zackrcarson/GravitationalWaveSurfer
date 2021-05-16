using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CreateGridWindow : EditorWindow
{
    float cameraHeight;
    float cameraWidth;

    int gridWidth = 20;
    int gridHeight = 14;
    float gridMarkerScale = 0.03f;

    GameObject gridMarker = null;
    GameObject gridMarkerParent = null;
    GameObject mainCanvas = null;

    [MenuItem("Window/Create Grid Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CreateGridWindow));
    }

    void OnGUI()
    {
        GUILayout.Label("Grid Settings", EditorStyles.boldLabel);
        gridWidth = EditorGUILayout.IntField("Grid Width", gridWidth);
        gridHeight = EditorGUILayout.IntField("Grid Height (even only)", gridHeight);
        gridMarkerScale = EditorGUILayout.FloatField("Grid Marker Scale", gridMarkerScale);

        if (gridHeight % 2 != 0)
        {
            gridHeight = gridHeight + 1;
        }

        gridMarker = (GameObject)(EditorGUILayout.ObjectField("Grid Marker Prefab", gridMarker, typeof(Object), true));
        gridMarkerParent = (GameObject)(EditorGUILayout.ObjectField("Grid Marker Parent Object", gridMarkerParent, typeof(Object), true));
        mainCanvas = (GameObject)(EditorGUILayout.ObjectField("Main Game Canvas", mainCanvas, typeof(Object), true));

        if (GUILayout.Button("Create Grid"))
        {
            

            DestroyGrid();
            CreateGrid();
        }

        if (GUILayout.Button("Destroy Grid"))
        {
            DestroyGrid();
        }
    }

    private void CreateGrid()
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