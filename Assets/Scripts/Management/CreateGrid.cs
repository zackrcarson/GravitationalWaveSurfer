using UnityEditor;
using System.Collections.Generic;
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
    [SerializeField] GameObject gridLineParent = null;
    [SerializeField] GameObject mainCanvas = null;
    
    [SerializeField] GameObject lineControllerPrefab = null;

    // Cached References
    float cameraHeight;
    float cameraWidth;

    void Start()
    {
        if (gridHeight % 2 == 0)
        {
            gridHeight += 1;
        }
        if (gridWidth % 2 == 0)
        {
            gridWidth += 1;
        }

        DestroyGrid();
        MakeGrid();
        ConnectGrid();
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

    private void ConnectGrid()
    {
        List<GridPoint> objsToConnect = new List<GridPoint>();

        int idx = 0;
        foreach (Transform child in gridMarkerParent.transform)
        {   
            int col = int.Parse(child.gameObject.name.Split('_')[1]);
            foreach (Transform grandchild in child)
            {
                int row = int.Parse(grandchild.gameObject.name.Split('_')[1]);

                GridPoint currentGridPoint = new GridPoint(row, col, grandchild.gameObject);
                //currentGridPoint.Describe();

                objsToConnect.Add(currentGridPoint);
            }
        }

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                int listPositionStart = i * gridHeight + j;
                GridPoint startingPoint = objsToConnect[listPositionStart];
                // Debug.Log((i, j, listPositionStart));
                // startingPoint.Describe();



                if (i != gridWidth - 1)
                {
                    int listPositionEndRight =  (i + 1) * gridHeight + j;
                    GridPoint endingPointRight = objsToConnect[listPositionEndRight];
                    // Debug.Log((i, j, listPositionEndRight));
                    // endingPointRight.Describe();

                    GameObject lineRight = Instantiate(lineControllerPrefab);
                    lineRight.transform.parent = gridLineParent.transform;
                    lineRight.gameObject.name = "line_right_(" + i.ToString() + "," + j.ToString() + ") -> (" + (i + 1).ToString() + "," + j.ToString() + ")";

                    LineController lineControllerRight = lineRight.GetComponent<LineController>();
                    lineControllerRight.SetupLine(startingPoint, endingPointRight);
                }



                if (j != gridHeight - 1)
                {
                    int listPositionEndUp =  i * gridHeight + (j + 1);
                    GridPoint endingPointUp = objsToConnect[listPositionEndUp];
                    // Debug.Log((i, j, listPositionEndUp));
                    // endingPointUp.Describe();

                    // Debug.Log(("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~", i, j));

                    GameObject lineUp = Instantiate(lineControllerPrefab);
                    lineUp.transform.parent = gridLineParent.transform;
                    lineUp.gameObject.name = "line_up_(" + i.ToString() + "," + j.ToString() + ") -> (" + i.ToString() + "," + (j + 1).ToString() + ")";

                    LineController lineControllerUp = lineUp.GetComponent<LineController>();
                    lineControllerUp.SetupLine(startingPoint, endingPointUp);
                }
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