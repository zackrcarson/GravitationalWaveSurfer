using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GWManagement : MonoBehaviour
{
    public Transform particlesParent; // Parent transform containing all particles
    public Vector3 blackHolePosition; // Position of the black hole (source of gravitational wave)
    public Vector3 waveEndpoint; // Endpoint of the gravitational wave
    public float frequency = 1f; // Frequency of the gravitational wave
    public float amplitude = 0.1f; // Amplitude of the gravitational wave
    public float speedOfLight = 1f; // Speed of light in your simulation scale
    public float planeSize = 10f; // Size of the visualization plane
    public float planeThickness = 0.1f; // Thickness of the visualization plane

    private Vector3 waveDirection;
    private float period;

    void Start()
    {
        period = 1f / frequency;
        waveDirection = (waveEndpoint - blackHolePosition).normalized;

        // Initialize all particles
        foreach (Transform particle in particlesParent)
        {
            Debug.Log(particle.name);
            InitializeParticle(particle);
        }

        CreateLine(blackHolePosition, waveEndpoint);
    }

    void Update()
    {
        float phase = (Time.time % period) / period;

        // Update all particles
        foreach (Transform particle in particlesParent)
        {
            ApplyPlusPolarization(particle, phase);
        }
    }

    void InitializeParticle(Transform particle)
    {
        ParticleData data = particle.gameObject.AddComponent<ParticleData>();
        data.initialPosition = particle.position;
        data.projectedPoint = ProjectPointOnLine(data.initialPosition, blackHolePosition, waveEndpoint);
    }

    void ApplyPlusPolarization(Transform particle, float phase)
    {
        ParticleData data = particle.GetComponent<ParticleData>();
        
        float s = 2 * Mathf.PI * phase;
        
        // Calculate the propagation delay
        float distanceAlongWave = Vector3.Dot(data.initialPosition - blackHolePosition, waveDirection);
        float zDelay = distanceAlongWave / speedOfLight;
        s -= 2 * Mathf.PI * (zDelay / period);

        // Calculate the displacement vector from the projected point
        Vector3 displacement = data.initialPosition - data.projectedPoint;

        // Calculate two perpendicular vectors in the plane
        Vector3 perp1 = Vector3.Cross(waveDirection, Vector3.up).normalized;
        Vector3 perp2 = Vector3.Cross(waveDirection, perp1).normalized;

        // Apply the plus polarization deformation in the plane
        Vector3 deformedDisplacement = 
            perp1 * (Vector3.Dot(displacement, perp1) * (1 + amplitude * Mathf.Cos(s))) +
            perp2 * (Vector3.Dot(displacement, perp2) * (1 - amplitude * Mathf.Cos(s))) +
            waveDirection * Vector3.Dot(displacement, waveDirection);

        // Calculate the final deformed position
        Vector3 deformedPosition = data.projectedPoint + deformedDisplacement;

        particle.position = deformedPosition;
    }

    Vector3 ProjectPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 lineDirection = (lineEnd - lineStart).normalized;
        float projection = Vector3.Dot(point - lineStart, lineDirection);
        return lineStart + projection * lineDirection;
    }

    private GameObject CreateLine(Vector3 start, Vector3 end, float lineWidth = 0.1f, Color color = default)
    {
        if (color == default)
        {
            color = Color.white;
        }

        // Create a cylinder
        GameObject lineObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        lineObject.name = "Line";

        // Get the line direction and length
        Vector3 lineDirection = (end - start);
        float lineLength = lineDirection.magnitude;

        // Set position to the midpoint between start and end
        lineObject.transform.position = (start + end) / 2f;

        // Scale the cylinder
        lineObject.transform.localScale = new Vector3(lineWidth, lineLength / 2f, lineWidth);

        // Rotate the cylinder to align with the line direction
        lineObject.transform.up = lineDirection.normalized;

        // Set the color of the line
        Renderer lineRenderer = lineObject.GetComponent<Renderer>();
        lineRenderer.material = new Material(Shader.Find("Standard"));
        lineRenderer.material.color = color;

        return lineObject;
    }

}

// Helper class to store particle-specific data
public class ParticleData : MonoBehaviour
{
    public Vector3 initialPosition;
    public Vector3 projectedPoint;
}