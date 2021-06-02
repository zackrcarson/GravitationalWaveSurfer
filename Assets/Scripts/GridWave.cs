using System.Collections.Generic;
using UnityEngine;

public class GridWave : MonoBehaviour
{
    // Config Parameters
    [SerializeField] bool manualControl = false;
    [SerializeField] bool manualControlFixedMass = false;
    [SerializeField] float manualControlFixedMass1 = 4.0f;
    [SerializeField] float manualControlFixedMass2 = 5.0f;
    [SerializeField] float maxDeviation = 0.5f;

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

    // State Variables
    float hOfTDelta;
    Vector3 deviationVector;
    public List<Vector3> sliceState;
    public bool isWaving = false;
    float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        timer = Random.Range(randomWaveOffTimeMin, randomWaveOffTimeMax);

        gravitationalWave = FindObjectOfType<GravitationalWave>();
        blackHoleDisplay = FindObjectOfType<BlackHoleDisplay>();

        stellarMinMass = blackHoleDisplay.stellarMinMass;
        stellarIntermediateMassBoundary = blackHoleDisplay.stellarIntermediateMassBoundary;
        intermediateSupermassiveMassBoundary = blackHoleDisplay.intermediateSupermassiveMassBoundary;
        supermassiveMaxMass = blackHoleDisplay.supermassiveMaxMass;

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
    }

    // Update is called once per frame
    void Update()
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

    private void ManualWaveControl()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ResetGridPositions();

            float[] masses;
            if (!manualControlFixedMass)
            {
                masses = RandomlyGenerateMasses();
                /*if (masses[1] == masses[0])
                {
                    masses[1] = 1.05f * masses[0];
                }*/
            }
            else
            {
                masses = new float[] { manualControlFixedMass1, manualControlFixedMass2 };
            }

            isWaving = !isWaving;
            blackHoleDisplay.DisplayBlackHoles(isWaving, masses[0], masses[1]);

            if (isWaving)
            {
                gravitationalWave.StartNewWave(masses[0], masses[1]);
                maxAmplitude = gravitationalWave.GetMaxAmplitude();
            }
        }
    }

    private void OperateWaveTimer()
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

                isWaving = true;

                blackHoleDisplay.DisplayBlackHoles(true, masses[0], masses[1]);
                gravitationalWave.StartNewWave(masses[0], masses[1]);
                maxAmplitude = gravitationalWave.GetMaxAmplitude();

                timer = Random.Range(randomWaveOnTimeMin, randomWaveOnTimeMax);
            }
        }
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

    private void WaveGrid()
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
}
