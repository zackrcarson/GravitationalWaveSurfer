using System.Collections.Generic;
using UnityEngine;

public class GridWave : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float maxDeviation = 0.5f;

    // Cached References
    List<List<Transform>> grid = null;
    List<List<Vector3>> gridOrigins = null;

    GravitationalWave gravitationalWave = null;

    int halfWayMarker = 0;
    float halfVerticalGridSpace = 0;
    float maxAmplitude = 0;

    // State Variables
    float hOfTDelta;
    Vector3 deviationVector;
    public List<Vector3> sliceState;

    // Start is called before the first frame update
    void Start()
    {
        gravitationalWave = FindObjectOfType<GravitationalWave>();
        deviationVector = new Vector3();
        sliceState = new List<Vector3>();

        CollectGrid();
    }

    private void CollectGrid()
    {
        grid = new List<List<Transform>>();
        gridOrigins = new List<List<Vector3>>();

        foreach (Transform child in transform)
        {
            List<Transform> slice = new List<Transform>();
            List<Vector3> sliceOrigins = new List<Vector3>();

            foreach (Transform grandChild in child)
            {
                slice.Add(grandChild);
                sliceOrigins.Add(grandChild.position);
            }

            grid.Add(slice);
            gridOrigins.Add(sliceOrigins);

            sliceState.Add(new Vector3(0f, 0f, 0f));
        }

        halfWayMarker = (int)(grid[0].Count / 2);
        halfVerticalGridSpace = (gridOrigins[0][1] - gridOrigins[0][0]).y * maxDeviation;
        maxAmplitude = gravitationalWave.GetMaxAmplitude();
    }

    // Update is called once per frame
    void Update()
    {
        hOfTDelta = gravitationalWave.GetGravitationalWave();
        deviationVector.y = hOfTDelta * (halfVerticalGridSpace / maxAmplitude);

        for (int k = sliceState.Count; k-- > 1;)
        {
            sliceState[k] = sliceState[k - 1];
        }
        sliceState[0] = deviationVector;

        int i = 0;
        foreach (List<Transform> slice in grid)
        {
            int j = 0;
            foreach (Transform child in slice)
            {
                if (j < halfWayMarker)
                {
                    child.position = gridOrigins[i][j] + sliceState[i];
                }
                else if (j > halfWayMarker)
                {
                    child.position = gridOrigins[i][j] - sliceState[i];
                }
                else
                {
                    child.position = gridOrigins[i][j];
                }

                j++;
            }

            i++;
        }
    }
}
