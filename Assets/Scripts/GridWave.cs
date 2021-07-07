using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridWave : MonoBehaviour
{
    // Config Parameters
    [Header("Manual Control Parameters")]
    [SerializeField] bool allowWaving = true;
    [SerializeField] bool manualControl = false;
    [SerializeField] bool manualControlFixedMass = false;
    [SerializeField] bool colorSliceNodes = false;
    [SerializeField] float manualControlFixedMass1 = 4.0f;
    [SerializeField] float manualControlFixedMass2 = 5.0f;
    [SerializeField] [Range(0f, 360f)] float manualControlFixedAngle = 92f;

    [Header("Gravitational Wave grid Mechanics")]
    [SerializeField] float maxDeviation = 0.5f;
    [SerializeField] float centerNoDeviationBuffer = 0.5f;
    [SerializeField] float[] numBuffersReducedDeviation = { 8f, 14f };
    [SerializeField] float[] nearbyReductionFactor = { 0.6f, 0.8f };
    [SerializeField] float cardinalAngleBlock = 0.1f;

    [Header("Random Timing Parameters")]
    [SerializeField] float randomWaveOffTimeMin = 1f;
    [SerializeField] float randomWaveOffTimeMax = 4f;
    [SerializeField] float randomWaveOnTimeMin = 5f;
    [SerializeField] float randomWaveOnTimeMax = 8f;

    [Header("Mass Probability Distribution")]
    [SerializeField] float stellarStellarProbability = 16.67f;
    [SerializeField] float intermediateIntermediateProbability = 16.67f;
    [SerializeField] float supermassiveSupermassiveProbability = 16.67f;
    [SerializeField] float stellarIntermediateProbability = 16.67f;
    [SerializeField] float stellarSupermassiveProbability = 16.67f;

    // Cached References
    BlackHoleDisplay blackHoleDisplay = null;
    GravitationalWave gravitationalWave = null;

    List<List<Transform>> grid = null;
    List<List<Vector3>> gridOrigins = null;
    List<List<float>> waveCenterDistances = null;

    float stellarMinMass = 3.8f;
    float stellarIntermediateMassBoundary = 100f;
    float intermediateSupermassiveMassBoundary = 100000.0f;
    float supermassiveMaxMass = 66000000000.0f;

    int halfWayMarker = 0;
    float halfVerticalGridSpace = 0;
    float maxAmplitude = 0;

    float bbhDisplayPanelOpeningTime = 1f;
    float bbhWarningMessageTime = 2f;

    float xMin, xMax, yMin, yMax;

    // State Variables
    float hOfTDelta;
    Vector3 deviationVector;
    Vector3 deviationDirection;
    float centerLineSlope;
    float[] wavefront;

    float distancePerSlice, numSlices;

    public List<Vector3> sliceState;
    List<List<int[]>> slices;

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

            /*sliceState.Add(new Vector3(0f, 0f, 0f));*/
        }

        halfWayMarker = (int)(grid[0].Count / 2);
        halfVerticalGridSpace = (gridOrigins[0][1] - gridOrigins[0][0]).y * maxDeviation;
    }

    // Update is called once per frame
    void Update()
    {
        if (allowWaving)
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
                    StartNewWave();
                }
            }
        }
    }

    public void StartNewWave()
    {
        float[] masses = RandomlyGenerateMasses();
        if (masses[1] == masses[0])
        {
            masses[1] = 1.05f * masses[0];
        }

        StartCoroutine(OpenBBHDisplayPanel(masses[0], masses[1]));
    }

    private IEnumerator OpenBBHDisplayPanel(float mass1, float mass2)
    {
        // Pick a random angle, and do all the calculations to collect proper slices that need to move with the angled GW
        float thetaGW = SetupArbitraryAngleGW();

        isWaitingForPanel = true;

        blackHoleDisplay.DisplayBlackHoles(true, mass1, mass2, thetaGW);

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

    private void ResetGridPositions()
    {
        if (grid[0][0] == null)
        {
            CollectGrid();
        }

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
        deviationVector = deviationDirection * hOfTDelta * (halfVerticalGridSpace / maxAmplitude);

        for (int k = sliceState.Count; k-- > 1;)
        {
            sliceState[k] = sliceState[k - 1];
        }
        sliceState[0] = deviationVector;

        int s = 0;
        foreach (List<int[]> slice in slices)
        {
            foreach (int[] node in slice)
            {
                int i = node[0];
                int j = node[1];

                float distanceFromCenterLine = waveCenterDistances[i][j];

                if (distanceFromCenterLine <= centerNoDeviationBuffer && distanceFromCenterLine >= - centerNoDeviationBuffer)
                {
                    grid[i][j].position = gridOrigins[i][j];
                }
                else if (distanceFromCenterLine > centerNoDeviationBuffer)
                {
                    float restriction = 1f;
                    if (distanceFromCenterLine < numBuffersReducedDeviation[0] * centerNoDeviationBuffer)
                    {
                        restriction = nearbyReductionFactor[0];
                    }
                    else if (distanceFromCenterLine >= numBuffersReducedDeviation[0] * centerNoDeviationBuffer && distanceFromCenterLine <= numBuffersReducedDeviation[1] * centerNoDeviationBuffer)
                    {
                        restriction = nearbyReductionFactor[1];
                    }

                    grid[i][j].position = gridOrigins[i][j] + sliceState[s] * restriction;
                }
                else if (distanceFromCenterLine < -centerNoDeviationBuffer)
                {
                    float restriction = 1f;
                    if (distanceFromCenterLine > -numBuffersReducedDeviation[0] * centerNoDeviationBuffer)
                    {
                        restriction = nearbyReductionFactor[0];
                    }
                    else if (distanceFromCenterLine <= numBuffersReducedDeviation[0] * centerNoDeviationBuffer && distanceFromCenterLine >= numBuffersReducedDeviation[1] * centerNoDeviationBuffer)
                    {
                        restriction = nearbyReductionFactor[1];
                    }

                    grid[i][j].position = gridOrigins[i][j] - sliceState[s] * restriction;
                }
                else
                {
                    grid[i][j].position = gridOrigins[i][j];
                }
            }

            s++;
        }
    }

    private float SetupArbitraryAngleGW()
    {
        // Pick a random angle for the GW to come from
        float thetaGW = PickGWAngle();

        // Find the wavefront, and get the perpendicular distance from each node to the wavefront line
        List<float[]> gridDistances = GetGridDistances(thetaGW);

        // Group nodes into wave slices. This splits up the total length (perp line to opposite corner perp distance) into equidistance slices, and separates the nodes into the slices they fit into.
        GetSlices(thetaGW, gridDistances);

        // Color the slices if wanted
        if (colorSliceNodes)
        {
            ColorSlices();
        }

        return thetaGW;
    }

    private float PickGWAngle()
    {
        // Pick a random angle, buffer it slightly if it's at exactly 0, 90, 180, 270, 260
        float thetaGW;
        if (manualControlFixedMass)
        {
            thetaGW = manualControlFixedAngle;
        }
        else
        {
            thetaGW = Random.Range(cardinalAngleBlock, 360f - cardinalAngleBlock);
        }

        if ((thetaGW >= 90f - cardinalAngleBlock && thetaGW <= 90f + cardinalAngleBlock) || (thetaGW >= 180f - cardinalAngleBlock && thetaGW <= 180f + cardinalAngleBlock) || (thetaGW >= 270f - cardinalAngleBlock && thetaGW <= 270f + cardinalAngleBlock))
        {
            thetaGW += cardinalAngleBlock;
        }

        return thetaGW;
    }

    private List<float[]> GetGridDistances(float thetaGW)
    {
        // Compute the equation (y=mx+b, written as [m,b] of the wavefront (perpendicular to wave vector), going through the corner of the screen in the pi/2 direction the wave came from
        wavefront = GetPerpFunction(thetaGW);

        // We want to deviate in the direction of the wavefront (so unity vector in the direction of the slope)
        deviationDirection = new Vector3(1f, wavefront[0], 0f);
        deviationDirection = Vector3.Normalize(deviationDirection);

        // Equation of the GW propogation direction passing through the center is y = y0/x0 x = tan(theta) x. This line will split which nodes go up, and which go down.
        centerLineSlope = Mathf.Tan(thetaGW * Mathf.Deg2Rad);

        // Get the perpendicular distance (i.e. closest distance) from the wavefront line (above) to the node for each node in the array. Order it from closest to furthest.
        List<float[]> gridDistances = new List<float[]>();
        waveCenterDistances = new List<List<float>>();

        int i = 0;
        foreach (List<Vector3> slice in gridOrigins)
        {
            waveCenterDistances.Add(new List<float>());

            int j = 0;
            foreach (Vector3 node in slice)
            {
                gridDistances.Add(new float[] { GetPerpDistance(node, wavefront), i, j });

                waveCenterDistances[i].Add(GetPerpDistance(node, new float[] {centerLineSlope , 0f}));
                if (node.y < centerLineSlope * node.x)
                {
                    waveCenterDistances[i][j] *= -1;
                }

                j++;
            }

            i++;
        }
        gridDistances = gridDistances.OrderBy(lst => lst[0]).ToList();
        return gridDistances;
    }

    private void GetSlices(float thetaGW, List<float[]> gridDistances)
    {
        // Count the number of slices there should be as the wave passes (= numColumns if coming from right/left, and = numRows if coming from top/bottom
        numSlices = 17;
        if ((thetaGW >= 315f || thetaGW <= 45) || (thetaGW > 135f && thetaGW <= 225f))
        {
            numSlices = grid.Count();
        }
        else if ((thetaGW > 45f && thetaGW <= 135f) || (thetaGW > 225f && thetaGW <= 315))
        {
            numSlices = grid[0].Count();
        }
        else
        {
            Debug.LogError("Incorrect angle " + thetaGW.ToString() + " found.");
        }

        // Group the nodes into evenly spaces distance slices parallel to the wavefront
        distancePerSlice = gridDistances.Last()[0] / numSlices;

        slices = new List<List<int[]>>();

        int i = 0;
        foreach (float[] node in gridDistances)
        {
            if ((node[0] > distancePerSlice * i && i != numSlices) || node[0] == 0)
            {
                i++;

                slices.Add(new List<int[]>());
                sliceState.Add(new Vector3(0f, 0f, 0f));
            }

            slices[i - 1].Add(new int[] { (int)node[1], (int)node[2] });
        }
    }

    private void ColorSlices()
    {
        Color[] colors = new Color[] { new Color(255f / 255f, 0f / 255f, 0f / 255f, 1f), new Color(255f / 255f, 127f / 255f, 0f / 255f, 1f), new Color(255f / 255f, 255f / 255f, 0f / 255f, 1f), new Color(0f / 255f, 255f / 255f, 0f / 255f, 1f), new Color(0f / 255f, 0f / 255f, 255f / 255f, 1f), new Color(75f / 255f, 0f / 255f, 130f / 255f, 1f), new Color(143f / 255f, 0f / 255f, 255f / 255f, 1f), new Color(255f / 255f, 255f / 255f, 255f / 255f, 1f) };

        int c = 0;
        foreach (List<int[]> slice in slices)
        {
            foreach (int[] node in slice)
            {
                grid[node[0]][node[1]].gameObject.GetComponent<SpriteRenderer>().color = colors[c];
            }
            c++;
            if (c > colors.Count() - 1) { c = 0; }
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

        // Pick the corner of the screen the wave is coming from
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

        // Slope and y-intercet of the wavefront passing through the corner point of the screen (math in docstring)
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
    float GetPerpDistance(Vector3 nodePoint, float[] perpLine)
    {
        // Perpendicular distance from the wavefront to the given node, math shown in docstring
        float x1 = (nodePoint.y + (nodePoint.x / perpLine[0]) - perpLine[1]) / (perpLine[0] + (1 / perpLine[0]));
        Vector3 perpLinePoint = new Vector3(x1, perpLine[0] * x1 + perpLine[1], 0f);

        return Vector3.Distance(nodePoint, perpLinePoint);
    }

    private void SetupScreenBoundaries()
    {
        Camera gameCamera = Camera.main;

        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
    }

    public Vector3 GetRiderDeviation(Vector3 riderPosition)
    {
        if (GetPerpDistance(riderPosition, new float[] { centerLineSlope , 0f }) < centerNoDeviationBuffer)
        {
            return Vector3.zero;
        }

        float topBottom = +1f;
        if (riderPosition.y < centerLineSlope * riderPosition.x)
        {
            topBottom = -1f;
        }

        int sliceNumber = Mathf.FloorToInt(GetPerpDistance(riderPosition, wavefront) / distancePerSlice);

        if (sliceNumber >= sliceState.Count)
        {
            sliceNumber = sliceState.Count - 1;
        }

        return topBottom *  sliceState[sliceNumber];
    }
}
