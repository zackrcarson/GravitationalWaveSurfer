using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridWave : MonoBehaviour
{
    // Config Parameters
    [SerializeField] bool manualControl = false;
    [SerializeField] bool manualControlFixedMass = false;
    [SerializeField] float manualControlFixedMass1 = 4.0f;
    [SerializeField] float manualControlFixedMass2 = 5.0f;
    [SerializeField] float maxDeviation = 0.5f;

    [SerializeField] float cardinalAngleBlock = 0.1f;

    [SerializeField] float randomWaveOffTimeMin = 1f;
    [SerializeField] float randomWaveOffTimeMax = 4f;

    [SerializeField] float randomWaveOnTimeMin = 5f;
    [SerializeField] float randomWaveOnTimeMax = 8f;

    [SerializeField] float stellarStellarProbability = 16.67f;
    [SerializeField] float intermediateIntermediateProbability = 16.67f;
    [SerializeField] float supermassiveSupermassiveProbability = 16.67f;
    [SerializeField] float stellarIntermediateProbability = 16.67f;
    [SerializeField] float stellarSupermassiveProbability = 16.67f;

    // Cached References
    float stellarMinMass = 3.8f;
    float stellarIntermediateMassBoundary = 100f;
    float intermediateSupermassiveMassBoundary = 100000.0f;
    float supermassiveMaxMass = 66000000000.0f;

    List<List<Transform>> grid = null;
    List<List<Vector3>> gridOrigins = null;

    BlackHoleDisplay blackHoleDisplay = null;
    GravitationalWave gravitationalWave = null;

    int halfWayMarker = 0;
    float halfVerticalGridSpace = 0;
    float maxAmplitude = 0;

    float bbhDisplayPanelOpeningTime = 1f;
    float bbhWarningMessageTime = 2f;

    float xMin, xMax, yMin, yMax;

    // State Variables
    float hOfTDelta;
    Vector3 deviationVector;
    public List<Vector3> sliceState;

    public bool isWaving = false;
    public bool canWave = true;
    bool isWaitingForPanel = false;

    float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        timer = Random.Range(randomWaveOffTimeMin, randomWaveOffTimeMax);

        gravitationalWave = FindObjectOfType<GravitationalWave>();

        blackHoleDisplay = FindObjectOfType<BlackHoleDisplay>();
        bbhDisplayPanelOpeningTime = blackHoleDisplay.panelMovementTime;
        bbhWarningMessageTime = blackHoleDisplay.warningMessageTime;

        stellarMinMass = blackHoleDisplay.stellarMinMass;
        stellarIntermediateMassBoundary = blackHoleDisplay.stellarIntermediateMassBoundary;
        intermediateSupermassiveMassBoundary = blackHoleDisplay.intermediateSupermassiveMassBoundary;
        supermassiveMaxMass = blackHoleDisplay.supermassiveMaxMass;

        deviationVector = new Vector3();
        sliceState = new List<Vector3>();

        CollectGrid();
        
        SetupScreenBoundaries();
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
    }

    // Update is called once per frame
    void Update()
    {
        if (canWave)
        {
            if (manualControl)
            {
                ManualWaveControl();
            }
            else
            {
                OperateWaveTimer();
            }

            if (isWaving)
            {
                WaveGrid();
            }
        }
    }

    public void AllowWaving(bool isAllowed)
    {
        canWave = isAllowed;
    }

    private void ManualWaveControl()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ResetGridPositions();

            float[] masses;
            if (!manualControlFixedMass)
            {
                masses = RandomlyGenerateMasses();
            }
            else
            {
                masses = new float[] { manualControlFixedMass1, manualControlFixedMass2 };
            }

            if (!isWaving)
            {
                StartCoroutine(OpenBBHDisplayPanel(masses[0], masses[1]));
            }
            else
            {
                isWaving = false;
                blackHoleDisplay.DisplayBlackHoles(false, masses[0], masses[1]);
            }
        }
    }

    public void StopWaving()
    {
        isWaving = false;
    }

    public void StartWaving()
    {
        isWaving = true;
    }

    private void OperateWaveTimer()
    {
        if (!isWaitingForPanel)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                ResetGridPositions();

                if (isWaving)
                {
                    isWaving = false;
                    blackHoleDisplay.DisplayBlackHoles(false);

                    timer = Random.Range(randomWaveOffTimeMin, randomWaveOffTimeMax);
                }
                else
                {
                    float[] masses = RandomlyGenerateMasses();
                    if (masses[1] == masses[0])
                    {
                        masses[1] = 1.05f * masses[0];
                    }

                    StartCoroutine(OpenBBHDisplayPanel(masses[0], masses[1]));
                }
            }
        }
    }

    private IEnumerator OpenBBHDisplayPanel(float mass1, float mass2)
    {
        isWaitingForPanel = true;

        blackHoleDisplay.DisplayBlackHoles(true, mass1, mass2);

        yield return new WaitForSeconds(bbhDisplayPanelOpeningTime + (2 * bbhWarningMessageTime));

        gravitationalWave.StartNewWave(mass1, mass2);
        maxAmplitude = gravitationalWave.GetMaxAmplitude();

        timer = Random.Range(randomWaveOnTimeMin, randomWaveOnTimeMax);

        isWaving = true;
        isWaitingForPanel = false;
    }

    private float[] RandomlyGenerateMasses()
    {
        float pickBlackHoleClass = Random.Range(0f, 100f);

        if (pickBlackHoleClass <= stellarStellarProbability)
        {
            return new float[] { Random.Range(stellarMinMass, stellarIntermediateMassBoundary), Random.Range(stellarMinMass, stellarIntermediateMassBoundary) };
        }
        else if (pickBlackHoleClass > stellarStellarProbability && pickBlackHoleClass <= (stellarStellarProbability + intermediateIntermediateProbability))
        {
            return new float[] { Random.Range(stellarIntermediateMassBoundary, intermediateSupermassiveMassBoundary), Random.Range(stellarIntermediateMassBoundary, intermediateSupermassiveMassBoundary) };
        }
        else if (pickBlackHoleClass > intermediateIntermediateProbability && pickBlackHoleClass <= (stellarStellarProbability + intermediateIntermediateProbability + supermassiveSupermassiveProbability))
        {
            return new float[] { Random.Range(intermediateSupermassiveMassBoundary, supermassiveMaxMass), Random.Range(intermediateSupermassiveMassBoundary, supermassiveMaxMass) };
        }
        else if (pickBlackHoleClass > supermassiveSupermassiveProbability && pickBlackHoleClass <= (stellarStellarProbability + intermediateIntermediateProbability + supermassiveSupermassiveProbability + stellarIntermediateProbability))
        {
            return new float[] { Random.Range(stellarMinMass, stellarIntermediateMassBoundary), Random.Range(stellarIntermediateMassBoundary, intermediateSupermassiveMassBoundary) };
        }
        else if (pickBlackHoleClass > stellarIntermediateProbability && pickBlackHoleClass <= (stellarStellarProbability + intermediateIntermediateProbability + supermassiveSupermassiveProbability + stellarIntermediateProbability + stellarSupermassiveProbability))
        {
            return new float[] { Random.Range(stellarMinMass, stellarIntermediateMassBoundary), Random.Range(intermediateSupermassiveMassBoundary, supermassiveMaxMass) };
        }
        else
        {
            return new float[] { Random.Range(stellarIntermediateMassBoundary, intermediateSupermassiveMassBoundary), Random.Range(intermediateSupermassiveMassBoundary, supermassiveMaxMass) };
        }
    }

    /*private void WaveGrid()
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
    }*/

    // Try multi-Dimensional

    private void ResetGridPositions()
    {
        int i = 0;
        foreach (List<Transform> slice in grid)
        {
            int j = 0;
            foreach (Transform child in slice)
            {
                if (j < halfWayMarker)
                {
                    child.position = gridOrigins[i][j];
                }
                else if (j > halfWayMarker)
                {
                    child.position = gridOrigins[i][j];
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














    private void WaveGrid()
    {
        hOfTDelta = gravitationalWave.GetGravitationalWave();
        deviationVector.y = hOfTDelta * (halfVerticalGridSpace / maxAmplitude);

        float theta = Random.Range(0f, 360f);
        float slope = Mathf.Tan(theta);



        
        
        
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

    private void SetupScreenBoundaries()
    {
        Camera gameCamera = Camera.main;

        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;


        float theta = Random.Range(cardinalAngleBlock, 360f - cardinalAngleBlock);
        if ((theta >= 90f - cardinalAngleBlock && theta <= 90f + cardinalAngleBlock) || (theta >= 180f - cardinalAngleBlock && theta <= 180f + cardinalAngleBlock) || (theta >= 270f - cardinalAngleBlock && theta <= 270f + cardinalAngleBlock))
        {
            theta += cardinalAngleBlock;
        }

        float[] perp = GetPerpFunction(theta);

        Debug.Log((theta, perp[0], perp[1]));

        List<float[]> gridDistances = new List<float[]>();
        int i = 0;
        foreach (List<Vector3> slice in gridOrigins)
        {
            int j = 0;
            foreach (Vector3 node in slice)
            {
                gridDistances.Add(new float[] { GetPerpDistance(node, perp), i, j });
                
                j++;
            }

            i++;
        }

        gridDistances = gridDistances.OrderBy(lst => lst[0]).ToList();

        float maxDistance = gridDistances.Last()[0];
        int numSlices = 17;
        if ((theta >= 315f || theta <= 45) || (theta > 135f && theta <= 225f))
        {
            numSlices = grid.Count();
        }
        else if ((theta > 45f && theta <= 135f) || (theta > 225f && theta <= 315))
        {
            numSlices = grid[0].Count();
        }
        else
        {
            Debug.LogError("Incorrect angle " + theta.ToString() + " found.");
        }
        int nodesPerSlice = grid[0].Count() * grid.Count() / numSlices;


        List<List<int[]>> slices = new List<List<int[]>>();

        // Group nodes into wave slices. This splits up the total length (perp line to opposite corner perp distance) into grid_width/grid_height equal length slices (depending on if it's coming from top/bottom or left/right side of the screen). Could potentially find a better looking way to do this :)
        i = 0;
        int k = -1;
        foreach (float[] node in gridDistances)
        {
            if (Mathf.FloorToInt(i / nodesPerSlice) > k)
            {
                k++;
                slices.Add(new List<int[]>());
            }

            slices[k].Add(new int[] { (int)node[1], (int)node[2] });

            Debug.Log((k, node[0], (int)node[1], (int)node[2]));
            i++;
        }
    }

    /// <summary>
    /// Returns [slope, y-intercept] to the line perpendicular to the GW angle, and passing through the correct corner of the screen:
    /// Equation for GW propagation: y = mx = y0 / x0 x = tan(theta) x (for any arbitrary y0, x0)
    /// Equation for perpendicular line: yp = -1 / m x + b = - 1 / tan(theta) x + b. Constrain to corner of screen (x', y') s.t. y' =  - 1 / tan(theta) x' + b => yp =  - 1 / tan(theta) x + y0 + x0 / tan(theta)
    /// See https://www.desmos.com/calculator/s71zjnlzed for example outputs for 25, 128, 223, and 330 degrees
    /// </summary>
    /// <returns></returns>
    private float[] GetPerpFunction(float angleGW)
    {
        float x0, y0, slope, intercept;

        if (angleGW >= 0f && angleGW <= 90f)
        {
            x0 = xMax;
            y0 = yMax;
        }
        else if (angleGW > 90f && angleGW <= 180f)
        {
            x0 = xMin;
            y0 = yMax;
        }
        else if (angleGW > 180f && angleGW <= 270f)
        {
            x0 = xMin;
            y0 = yMin;
        }
        else if (angleGW > 270f && angleGW <= 360f)
        {
            x0 = xMax;
            y0 = yMin;
        }
        else
        {
            Debug.LogError("Incorrect angle " + angleGW.ToString() + " found.");
            return new float[] { 0f, 0f };
        }

        slope = - 1 / (Mathf.Tan(angleGW * Mathf.Deg2Rad));
        intercept = y0 + (x0 / (Mathf.Tan(angleGW * Mathf.Deg2Rad)));

        return new float[] { slope, intercept };
    }

    /// <summary>
    /// Returns the perpendicular distance from the GW perp line to a given node point (i.e. the closest distance to the line).
    /// perp line equation: y = mx+b (perpLine[0,1] for m, b)
    /// Equation of line perpendicular to that: yp = -1/mx + bp
    /// Constrain to go through the node point <x0, y0>: y0 = -1/mx0 + bp => bp = y0 + x0/m => yp = -1/m x + y0 + x0/m
    /// Set them equal to find intersection point <x1, y1>: y = mx1+b = yp = -1/m x1 + y0 + x0/m => x1 = (y0 + x0/m - b)/(m + 1/m) => y1 = m x1 + b
    /// Distance is the distance from the node point <x0, y0> and the intersection point <x1, y1>
    /// </summary>
    /// <param name="nodePoint"></param>
    /// <param name="perpLine"></param>
    /// <returns></returns>
    private float GetPerpDistance(Vector3 nodePoint, float[] perpLine)
    {
        float x1 = (nodePoint.y + (nodePoint.x / perpLine[0]) - perpLine[1]) / (perpLine[0] + (1 / perpLine[0]));
        Vector3 perpLinePoint = new Vector3(x1, perpLine[0] * x1 + perpLine[1], 0f);

        return Vector3.Distance(nodePoint, perpLinePoint);
    }
}
