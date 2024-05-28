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

/*
need a list, can delete one by one

*/
    private List<GameObject> protonParticles;
    private List<GameObject> neutronParticles;
    private List<Rigidbody> protonRigidbodies;
    private List<Rigidbody> neutronRigidbodies;

    void Start()
    {
        if (folder == null)
        {
            folder = new GameObject(folderName);
        }
        
        protonParticles = new List<GameObject>();
        neutronParticles = new List<GameObject>();
        protonRigidbodies = new List<Rigidbody>();
        neutronRigidbodies = new List<Rigidbody>();

        numInNucleus = numProton + numNeutron;

        StartCoroutine(Simulate(true));
    }

    public void StartSimulation(bool initialize)
    {
        StartCoroutine(Simulate(initialize));
    }

    IEnumerator Simulate(bool initialize)
    {
        if (initialize) InitializeNucleus();
        Physics.gravity = Vector3.zero; // Disable default gravity

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            bool hasConverged = true;

            // Update gravitational force for each particle in the nucleus
            foreach (Rigidbody rb in protonRigidbodies)
            {
                // Gravity: inward direction towards origin
                Vector3 directionToOrigin = -rb.position.normalized;
                rb.AddForce(directionToOrigin * gravitationalConstant, ForceMode.Acceleration);
                rb.velocity *= dampingFactor;   // Manual damping
            }
            foreach (Rigidbody rb in neutronRigidbodies)
            {
                // Gravity: inward direction towards origin
                Vector3 directionToOrigin = -rb.position.normalized;
                rb.AddForce(directionToOrigin * gravitationalConstant, ForceMode.Acceleration);
                // Manually take away energy to help convergence
                rb.velocity *= dampingFactor;   // Manual damping
            }

            yield return new WaitForFixedUpdate(); // Wait for physics update

            // Check for convergence
            foreach (Rigidbody rb in protonRigidbodies)
            {
                if (rb.velocity.magnitude > threshold) hasConverged = false;
            }
            foreach (Rigidbody rb in neutronRigidbodies)
            {
                if (rb.velocity.magnitude > threshold) hasConverged = false;
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

    void InitializeNucleus()
    {
        Debug.Log("Initializing nucleus.");

        for (int i = 0; i < numInNucleus; i++)
        {
            if (i < numNeutron)
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
        if (type == "Neutron") 
        {
            particle = Instantiate(neutronPrefab, spawnPosition, Quaternion.identity);

            particle.name = $"Neutron_{i}"; // rename for cleanliness
            particle.transform.parent = folder.transform; // all particles in a folder for cleanliness

            neutronParticles.Add(particle);

            // Get Rigidbody
            Rigidbody rb = particle.GetComponent<Rigidbody>();
            neutronRigidbodies.Add(rb);

            // Apply an initial random velocity to kickstart convergence
            Vector3 initialVelocity = Random.insideUnitSphere * initialVelocityRange;
            rb.velocity = initialVelocity;            
        }
        else if (type == "Proton") 
        {
            particle = Instantiate(protonPrefab, spawnPosition, Quaternion.identity);

            particle.name = $"Proton_{i}"; // rename for cleanliness
            particle.transform.parent = folder.transform; // all particles in a folder for cleanliness

            protonParticles.Add(particle);

            // Get Rigidbody
            Rigidbody rb = particle.GetComponent<Rigidbody>();
            protonRigidbodies.Add(rb);

            // Apply an initial random velocity to kickstart convergence
            Vector3 initialVelocity = Random.insideUnitSphere * initialVelocityRange;
            rb.velocity = initialVelocity;
        }
        else {
            Debug.LogWarning("Invalid type of particle!"); 
            return;
        }
    }

    public void AddParticle(string type)
    {
        Debug.Log($"Adding {type}...");
        if (type == "Proton") numProton++;
        else if (type == "Neutron") numNeutron++;
        else
        {
            Debug.LogWarning("Invalid particle type!!");
            return;
        }
        numInNucleus++;
        SpawnParticle(type, numInNucleus-1);
        StartAllMovement();
        StartSimulation(false);
    }

    public void RemoveParticle(string type)
    {
        List<GameObject> particleList;
        if (type == "Neutron") particleList = neutronParticles;
        else if (type == "Proton") particleList = protonParticles;
        else
        {
            Debug.LogWarning("Invalid type of particle!");
            return;
        }

        if (particleList.Count == 0)
        {
            Debug.LogWarning($"No {type} particles left to remove!");
            return;
        }

        if (type == "Neutron")
        {
            int lastIndex = neutronParticles.Count - 1;

            GameObject particleToRemove = neutronParticles[lastIndex];
            Rigidbody rbToRemove = neutronRigidbodies[lastIndex];

            neutronParticles.RemoveAt(lastIndex);
            neutronRigidbodies.RemoveAt(lastIndex);

            Destroy(particleToRemove);
            Destroy(rbToRemove);

            numNeutron--;
        }
        else if (type == "Proton")
        {
            int lastIndex = protonParticles.Count - 1;

            GameObject particleToRemove = protonParticles[lastIndex];
            Rigidbody rbToRemove = protonRigidbodies[lastIndex];

            protonParticles.RemoveAt(lastIndex);
            protonRigidbodies.RemoveAt(lastIndex);

            Destroy(particleToRemove);
            Destroy(rbToRemove);

            numProton--;
        }

        numInNucleus--;

        Debug.Log($"Removed {type}.");    

        StartAllMovement();
        StartSimulation(false);
    }

    void StartAllMovement()
    {
        foreach (Rigidbody rb in protonRigidbodies)
        {
            rb.isKinematic = false;
        }
        foreach (Rigidbody rb in neutronRigidbodies)
        {
            rb.isKinematic = false;
        }
    }

    void StopAllMovement()
    {
        foreach (Rigidbody rb in protonRigidbodies)
        {
            rb.isKinematic = true;
        }
        foreach (Rigidbody rb in neutronRigidbodies)
        {
            rb.isKinematic = true;
        }
    }
}
