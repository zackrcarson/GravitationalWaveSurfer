using System.Collections.Generic;
using UnityEngine;

public class WaveRider : MonoBehaviour
{
    // Config Parameters
    [SerializeField] public bool canRide = true;
    [SerializeField] float ridePercent = 0.2f;
    [SerializeField] float bufferPercentage = 0.07f;

    // Cached Refences
    GravitationalWave gravitationalWave = null;
    GridWave gridWave = null;
    float xMin, xMax, halfTotal, gridSpacing, yBuffer;

    // State Variables
    List<Vector3> sliceState;

    // Start is called before the first frame update
    void Start()
    {
        SetupPlayAreaBoundaries();

        gravitationalWave = FindObjectOfType<GravitationalWave>();
        gridWave = FindObjectOfType<GridWave>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canRide)
        {
            if (gridWave.isWaving)
            {
                sliceState = gridWave.sliceState;

                Vector3 deviation = gridWave.GetRiderDeviation(transform.position);

                transform.position += ridePercent * deviation;
            }    
        }
    }

    public void AllowRiding(bool isAllowed)
    {
        canRide = isAllowed;
    }

    private void SetupPlayAreaBoundaries()
    {
        Camera gameCamera = Camera.main;

        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        float yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        yBuffer = yMax * bufferPercentage;

        halfTotal = (xMax - xMin) / 2.0f;

        int gridWidth = FindObjectOfType<CreateGrid>().gridWidth - 1;
        gridSpacing = (xMax - xMin) / (float)gridWidth;
    }
}
