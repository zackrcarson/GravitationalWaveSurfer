using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimulationManagement : MonoBehaviour
{
    public int numElectron = 10;
    public int numProton = 10;
    public int numNeutron = 10;
    private int numInNucleus;
    public GameObject electronPrefab;
    public GameObject neutronPrefab;
    public GameObject protonPrefab;
    public GameObject folder; // Folder that holds all generated particles
    public string folderName = "Nucleus";

    public float gravitationalConstant = 50f;
    public float threshold = 1e-3f;
    public int maxIterations = 10000;
    public float initialVelocityRange = 1f; // Initial random velocity range
    public float spawnSphereRadius = 5f; // Radius of the spawn sphere
    public float dampingFactor = 0.98f; // Factor to manually decrease the velocity

    private List<GameObject> particles;
    private List<Rigidbody> sphereRigidbodies;

    void Start()
    {
        if (folder == null)
        {
            folder = new GameObject(folderName);
        }

        numInNucleus = numProton + numNeutron;

        StartCoroutine(Simulate());
    }

    public void StartSimulation()
    {
        StartCoroutine(Simulate());
    }

    void InitializeNucleus()
    {
        particles = new List<GameObject>();
        sphereRigidbodies = new List<Rigidbody>();

        for (int i = 0; i < numInNucleus; i++)
        {
            if (i > numNeutron)
            {
                SpawnParticle("Neutron", i);
            }
            else
            {
                SpawnParticle("Proton", i);
            }
        }
    }

    void SpawnParticle(string type, int i)
    {
        Vector3 randomDirection = Random.onUnitSphere; // Point on the surface of a unit sphere
        Vector3 spawnPosition = randomDirection * spawnSphereRadius; // Scale to the desired radius

        // Instantiate particle objects
        GameObject particle;
        if (type == "Neutron") particle = Instantiate(neutronPrefab, spawnPosition, Quaternion.identity);
        else if (type == "Proton") particle = Instantiate(protonPrefab, spawnPosition, Quaternion.identity);
        else {
            Debug.LogWarning("Invalid type of particle!"); 
            return;
        }

        particle.name = $"{type}_{i}"; // rename for cleanliness
        particle.transform.parent = folder.transform; // all particles in a folder for cleanliness

        particles.Add(particle);

        // Get Rigidbody
        Rigidbody rb = particle.GetComponent<Rigidbody>();
        sphereRigidbodies.Add(rb);

        // Apply an initial random velocity to kickstart convergence
        Vector3 initialVelocity = Random.insideUnitSphere * initialVelocityRange;
        rb.velocity = initialVelocity;
    }

    IEnumerator Simulate()
    {
        InitializeNucleus();
        Physics.gravity = Vector3.zero; // Disable default gravity

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            bool hasConverged = true;

            // Update gravitational force for each particle in the nucleus
            foreach (Rigidbody rb in sphereRigidbodies)
            {
                // Gravity: inward direction towards origin
                Vector3 directionToOrigin = -rb.position.normalized;
                rb.AddForce(directionToOrigin * gravitationalConstant, ForceMode.Acceleration);

                // Manually take away energy to help convergence
                rb.velocity *= dampingFactor;
            }

            yield return new WaitForFixedUpdate(); // Wait for physics update

            // Check for convergence
            foreach (Rigidbody rb in sphereRigidbodies)
            {
                if (rb.velocity.magnitude > threshold)
                {
                    hasConverged = false;
                }
            }

            if (hasConverged)
            {
                Debug.Log("Simulation has converged.");
                StopAllMovement();
                yield break;
            }
        }

        StopAllMovement();
        Debug.Log("Max iterations reached without convergence.");
    }

    void StopAllMovement()
    {
        foreach (Rigidbody rb in sphereRigidbodies)
        {
            rb.isKinematic = true;
        }
    }

    void StartAllMovement()
    {
        foreach (Rigidbody rb in sphereRigidbodies)
        {
            rb.isKinematic = false;
        }
    }

    public void AddProton()
    {
        Debug.Log("Adding proton...");
        numProton++;
        numInNucleus++;
        SpawnParticle("Proton", numInNucleus-1);
        StartAllMovement();
        Simulate();
    }

    public void AddNeutron()
    {
        Debug.Log("Adding neutron...");
        numNeutron++;
        numInNucleus++;
        SpawnParticle("Neutron", numInNucleus-1);
        StartAllMovement();
        Simulate();
    }
}
