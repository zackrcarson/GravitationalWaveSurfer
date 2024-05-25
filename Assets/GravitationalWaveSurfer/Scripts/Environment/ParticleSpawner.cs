// File: Assets/Scripts/Environment/ParticleSpawner.cs

using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public GameObject particlePrefab; // Prefab of the particle system
    public GameObject particleFolder; // Folder that holds all generated particles
    public string folderName = "Particle Folder";
    public float chunkSize = 20f;         // Size of the spawn chunk
    public int numberOfParticles = 2000;  // Number of particles to spawn
    public int numberOfClusters = 20;     // Number of clusters
    public float clusterRadius = 20f;     // Radius of each cluster
    public float clusterCenterPadding = 5f; // Cluster Center must be this number far away from boundary
    public float minDistanceFromOrigin = 1.5f; // Minimum distance away from base
    private Vector3 origin = new Vector3(0, 0, 0);

    void Start()
    {
        particleFolder = new GameObject(folderName);
        SpawnParticles();
    }

    void SpawnParticles()
    {
        // Generate cluster centers
        Vector3[] clusterCenters = new Vector3[numberOfClusters];
        for (int i = 0; i < numberOfClusters; i++)
        {
            clusterCenters[i] = GetRandomPositionInCube(clusterCenterPadding);
        }

        // Distribute particles around cluster centers
        for (int i = 0; i < numberOfParticles; i++)
        {
            Vector3 particlePosition;
            do
            {
                Vector3 clusterCenter = clusterCenters[Random.Range(0, numberOfClusters)];
                Vector3 randomOffset = GetRandomPositionInSphere(clusterRadius);
                particlePosition = clusterCenter + randomOffset;
            }
            while (Vector3.Distance(particlePosition, origin) < minDistanceFromOrigin);

            GameObject particle = Instantiate(particlePrefab, particlePosition, Quaternion.identity);
            particle.transform.parent = particleFolder.transform;
        }
    }

    Vector3 GetRandomPositionInCube(float clusterCenterPadding)
    {
        float halfSize = chunkSize / 2f - clusterCenterPadding;
        float x = Random.Range(-halfSize, halfSize);
        float y = Random.Range(-halfSize, halfSize);
        float z = Random.Range(-halfSize, halfSize);
        return new Vector3(x, y, z);
    }

    Vector3 GetRandomPositionInSphere(float radius)
    {
        return Random.insideUnitSphere * radius;
    }
}
