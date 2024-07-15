using System.Collections.Generic;
using UnityEngine;

namespace GWS.WorldGen
{
    public class ParticleSpawner : MonoBehaviour
    {
        public static ParticleSpawner Instance { get; private set; }

        public GameObject particlePrefab; // Prefab of the particle system

        public int numParticles = 2000;  // Number of particles to spawn
        public int numClusters = 20;     // Number of clusters
        public float clusterRadius = 20f;     // Radius of each cluster
        public float clusterCenterPadding = 5f; // Cluster Center must be this number far away from boundary
        public float minDistanceFromOrigin = 1.5f; // Minimum distance away from base

        private void Awake() {
            if (Instance == null)
            {
                Instance = this;
                // DontDestroyOnLoad(gameObject);
            }
            else { Destroy(gameObject); }
        }

        /// <summary>
        /// Main generative function, right now only generating particles<br />
        /// </summary>
        /// <param name="chunkPosition"></param>
        /// <param name="chunkSize"></param>
        /// <returns>List of ParticleData</returns>
        public List<Vector3> GenerateObjectsForChunk(Vector3Int chunkPosition, float chunkSize)
        {
            List<Vector3> particles = new List<Vector3>();
            Vector3 chunkCenter = new Vector3(chunkPosition.x * chunkSize + chunkSize / 2,
                                              chunkPosition.y * chunkSize + chunkSize / 2,
                                              chunkPosition.z * chunkSize + chunkSize / 2);

            // Generate cluster centers
            Vector3[] clusterCenters = new Vector3[numClusters];
            for (int i = 0; i < numClusters; i++)
            {
                clusterCenters[i] = chunkCenter + GetRandomPositionInChunk(chunkSize, clusterCenterPadding);
            }

            // Generate particles around cluster centers
            for (int i= 0; i < numParticles; i++)
            {
                Vector3 particlePosition;
                do
                {
                    Vector3 clusterCenter = clusterCenters[Random.Range(0, numClusters)];
                    Vector3 randomOffset = GetRandomPositionInSphere(clusterRadius);
                    particlePosition = clusterCenter + randomOffset;
                }
                while (!IsPositionInChunk(particlePosition, chunkCenter, chunkSize));

                particles.Add(particlePosition);
            }

            return particles;
        }

        /// <summary>
        /// Helper function: random position within a chunk of given size with center at origin
        /// </summary>
        /// <param name="chunkSize"></param>
        /// <param name="clusterCenterPadding">Distance from edge of chunk</param>
        /// <returns></returns>
        Vector3 GetRandomPositionInChunk(float chunkSize, float clusterCenterPadding)
        {
            float halfSize = chunkSize / 2f - clusterCenterPadding;
            float x = Random.Range(-halfSize, halfSize);
            float y = Random.Range(-halfSize, halfSize);
            float z = Random.Range(-halfSize, halfSize);
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Helper function: random position within sphere of given radius
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        Vector3 GetRandomPositionInSphere(float radius)
        {
            return Random.insideUnitSphere * radius;
        }

        /// <summary>
        /// Helper function: checks if position is within a chunk
        /// </summary>
        /// <param name="position">Position to be checked</param>
        /// <param name="chunkCenter">Center of a chunk</param>
        /// <param name="chunkSize">Size of a chunk</param>
        /// <returns></returns>
        bool IsPositionInChunk(Vector3 position, Vector3 chunkCenter, float chunkSize)
        {
            float half = chunkSize / 2;
            return position.x >= chunkCenter.x - half && position.x < chunkCenter.x + half &&
                   position.y >= chunkCenter.y - half && position.y < chunkCenter.y + half &&
                   position.z >= chunkCenter.z - half && position.z < chunkCenter.z + half;
        }

    }
}