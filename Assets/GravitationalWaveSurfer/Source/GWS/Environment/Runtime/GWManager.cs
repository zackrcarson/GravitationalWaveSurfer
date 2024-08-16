using System.Collections.Generic;
using System;
using UnityEngine;

using GWS.Data;
using GWS.WorldGen;
using System.Linq;

public class GWManager : MonoBehaviour
{
    public static GWManager Instance { get; private set; }

    [Header("GameObject Information")]
    public Transform player;
    public Vector3 blackHolePosition; // Position of the black hole (source of gravitational wave)

    [Space(6)]
    [Header("Randomness Contorls")]
    public float minInitialFrequency = 0.3f;
    public float maxInitialFrequency = 1.5f;
    public float minInitialAmplitude = 0.01f;
    public float maxInitialAmplitude = 0.1f;
    public float minMergeTime = 10f;
    public float maxMergeTime = 20f;

    [Space(6)]
    [Header("Wave Properties")]
    public float speedOfLight = 8f; // Speed of light in your simulation scale
    public float initialFrequency = 1f; // Initial frequency of the gravitational wave
    public float initialAmplitude = 1f; // Amplitude of the gravitational wave
    public float mergeTime = 5f; // Time at which the merger occurs
    public float peakFrequency = 1f; // Maximum frequency at merger
    public float peakAmplitude = 2f; // Maximum amplitude at merger
    public float postMergerDecayRate = 2f; // Faster decay rate after merger

    private HashSet<Chunk> initializedChunks = new HashSet<Chunk>();
    private List<ParticleData> particleDataList = new List<ParticleData>();
    private GWData gWData;
    private Vector3 playerPosition; // Endpoint of the gravitational wave
    private Vector3 waveDirection;
    private float elapsedTime;

    [Space(6)]
    [Header("GW Generation Controls")]
    public float minTimeBetweenWaves = 60f;
    public float maxTimeBetweenWaves = 120f;
    public float nextWaveTime;
    private float timeSinceLastWave;
    public bool IsWaveActive { get; private set; }

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        playerPosition = player.position;
        IsWaveActive = false;

        SetNextWaveTime();

        // TriggerGravitationalWave();
        // CreateLine(blackHolePosition, playerPosition);
    }

    void Update()
    {
        timeSinceLastWave += Time.deltaTime;

        if (timeSinceLastWave >= nextWaveTime)
        {
            CreateGWData();
            TriggerGravitationalWave();
            SetNextWaveTime();
        }

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     TriggerGravitationalWave();
        // }

        if (IsWaveActive)
        {
            UpdateGravitationalWave();
        }
    }

    void SetNextWaveTime()
    {
        nextWaveTime = UnityEngine.Random.Range(minTimeBetweenWaves, maxTimeBetweenWaves);
        timeSinceLastWave = 0f;
    }

    void CreateGWData()
    {
        Vector3 destPos = GenerateRandomDistantPosition(playerPosition);
        GWData data = new GWData(destPos, playerPosition)
        {
            initialAmplitude = UnityEngine.Random.Range(minInitialAmplitude, maxInitialAmplitude),
            initialFrequency = UnityEngine.Random.Range(minInitialFrequency, maxInitialFrequency),
            peakAmplitude = 2f * initialAmplitude,
            peakFrequency = 2f * initialFrequency,
            mergeTime = UnityEngine.Random.Range(minMergeTime, maxMergeTime) + Vector3.Distance(destPos, playerPosition) / speedOfLight
        };

        gWData = data;
    }

    public void TriggerGravitationalWave()
    {
        InitializeAllActiveChunks();
        waveDirection = (playerPosition - blackHolePosition).normalized;
        elapsedTime = 0f;
        IsWaveActive = true;
        Debug.Log("Gravitational Wave Triggered!");
    }

    private void InitializeAllActiveChunks()
    {
        initializedChunks.Clear();
        var activeChunks = ChunkManager.Instance.GetActiveChunks();

        foreach (Chunk chunk in activeChunks)
        {
            InitializeChunk(chunk);
        }

    }

    public void InitializeChunk(Chunk chunk)
    {
        if (!IsWaveActive || initializedChunks.Contains(chunk)) return;

        foreach (Transform particle in chunk.ChunkObject.transform)
        {
            InitializeParticle(particle);
        }
        initializedChunks.Add(chunk);
    }

    private void InitializeParticle(Transform particle)
    {
        ParticleData data = particle.GetComponent<ParticleData>();
        if (data == null)
        {
            data = particle.gameObject.AddComponent<ParticleData>();
        }        

        data.initialPosition = particle.position;
        data.projectedPoint = ProjectPointOnLine(data.initialPosition, gWData.sourcePosition, gWData.destinationPosition);
        data.propagationDelay = Vector3.Distance(data.projectedPoint, gWData.sourcePosition) / speedOfLight;
        float radius = Vector3.Distance(data.initialPosition, data.projectedPoint);
        data.amplitudeScale = 1 / (float)Math.Sqrt(radius);
    }

    void UpdateGravitationalWave()
    {
        elapsedTime += Time.deltaTime;

        var activeChunks = ChunkManager.Instance.GetActiveChunks();

        foreach (Chunk chunk in activeChunks)
        {
            // POIs are unaffected by GW
            if (chunk.HasPOI) continue;

            if (!initializedChunks.Contains(chunk))
            {
                InitializeChunk(chunk);
            }

            foreach (Transform particle in chunk.ChunkObject.transform)
            {
                ParticleData data = particle.GetComponent<ParticleData>();
                if (data != null)
                {
                    ApplyPlusPolarization(data, elapsedTime);
                }
            }
        }

        if (elapsedTime > gWData.mergeTime + 5f) // Allow some time for post-merger effects
        {
            IsWaveActive = false;
            initializedChunks.Clear();
            Debug.Log("GW Ended!");
        }
    }

    void InitializeParticles()
    {
        foreach (Transform chunkTransform in ChunkManager.Instance.chunkParent.transform)
        {
            if (chunkTransform.gameObject.activeSelf)
            {
                foreach (Transform particle in chunkTransform)
                {
                    ParticleData data = particle.gameObject.AddComponent<ParticleData>();
                    data.initialPosition = particle.position;
                    data.projectedPoint = ProjectPointOnLine(data.initialPosition, gWData.sourcePosition, gWData.destinationPosition);
                    data.propagationDelay = Vector3.Distance(data.projectedPoint, gWData.sourcePosition) / speedOfLight;
                    float radius = Vector3.Distance(data.initialPosition, data.projectedPoint);
                    data.amplitudeScale = 1 / (float)Math.Sqrt(radius);
                    particleDataList.Add(data);
                }
            }
        }
    }

    void ApplyPlusPolarization(ParticleData data, float globalElapsedTime)
    {
        initialAmplitude = gWData.initialAmplitude;
        initialFrequency = gWData.initialFrequency;
        peakAmplitude = gWData.peakAmplitude;
        peakFrequency = gWData.peakFrequency;
        mergeTime = gWData.mergeTime;
        postMergerDecayRate = gWData.postMergerDecayRate;

        data.particleElapsedTime = globalElapsedTime - data.propagationDelay;

        if (data.particleElapsedTime < 0)
        {
            // Wave hasn't reached this particle yet
            data.transform.position = data.initialPosition;
            return;
        }

        float t = data.particleElapsedTime / mergeTime; 

        float particleAmplitude, particleFrequency;

        if (t <= 1)
        {
            // Pre-merge and merge: smooth increase to peak
            // particleAmplitude = data.amplitudeScale * Mathf.Lerp(initialAmplitude, peakAmplitude, t);
            particleAmplitude = Mathf.Lerp(initialAmplitude, peakAmplitude, t);
            particleFrequency = Mathf.Lerp(initialFrequency, peakFrequency, t);
        }
        else
        {
            // Post-merge: smooth decay
            float timeSinceMerge = data.particleElapsedTime - mergeTime;
            float decayFactor = Mathf.Exp(-postMergerDecayRate * timeSinceMerge);
            // particleAmplitude = data.amplitudeScale * peakAmplitude * decayFactor;
            particleAmplitude = peakAmplitude * decayFactor;
            particleFrequency = peakFrequency * decayFactor;
        }

        float adjustedPhase = 2 * Mathf.PI * (data.particleElapsedTime * particleFrequency);
        float s = Mathf.Sin(adjustedPhase);

        // Calculate the displacement vector from the projected point
        Vector3 displacement = data.initialPosition - data.projectedPoint;

        // Calculate two perpendicular vectors in the plane
        Vector3 perp1 = Vector3.Cross(waveDirection, Vector3.up).normalized;
        Vector3 perp2 = Vector3.Cross(waveDirection, perp1).normalized;

        // Apply the plus polarization deformation in the plane
        Vector3 deformedDisplacement = 
            perp1 * (Vector3.Dot(displacement, perp1) * (1 + data.amplitudeScale * initialAmplitude * s)) +
            perp2 * (Vector3.Dot(displacement, perp2) * (1 - data.amplitudeScale * initialAmplitude * s)) +
            waveDirection * Vector3.Dot(displacement, waveDirection);

        data.transform.position = data.projectedPoint + deformedDisplacement;
    }

    Vector3 GenerateRandomDistantPosition(Vector3 centerPosition)
    {
        float minDistance = ChunkManager.Instance.chunkSize * (ChunkManager.Instance.renderDistance + 2);

        while (true)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere.normalized; // random direction
            float randomDistance = minDistance + UnityEngine.Random.value * 50f; // within 50f of minDistance, can be modified
            Vector3 newPosition = centerPosition + randomDirection * randomDistance;

            if (Vector3.Distance(newPosition, centerPosition) >= minDistance)
            {
                return newPosition;
            }
        }
    }

    Vector3 ProjectPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 lineDirection = (lineEnd - lineStart).normalized;
        float projection = Vector3.Dot(point - lineStart, lineDirection);
        return lineStart + projection * lineDirection;
    }

    // private GameObject CreateLine(Vector3 start, Vector3 end, float lineWidth = 0.1f, Color color = default)
    // {
    //     if (color == default)
    //     {
    //         color = Color.white;
    //     }

    //     // Create a cylinder
    //     GameObject lineObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
    //     lineObject.name = "Line";

    //     // Get the line direction and length
    //     Vector3 lineDirection = (end - start);
    //     float lineLength = lineDirection.magnitude;

    //     // Set position to the midpoint between start and end
    //     lineObject.transform.position = (start + end) / 2f;

    //     // Scale the cylinder
    //     lineObject.transform.localScale = new Vector3(lineWidth, lineLength / 2f, lineWidth);

    //     // Rotate the cylinder to align with the line direction
    //     lineObject.transform.up = lineDirection.normalized;

    //     // Set the color of the line
    //     Renderer lineRenderer = lineObject.GetComponent<Renderer>();
    //     lineRenderer.material = new Material(Shader.Find("Standard"));
    //     lineRenderer.material.color = color;

    //     return lineObject;
    // }

}