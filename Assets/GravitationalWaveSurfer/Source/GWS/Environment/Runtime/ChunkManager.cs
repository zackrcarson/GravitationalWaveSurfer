using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GWS.WorldGen
{
    /// <summary>
    /// Chunk generation and update, helper functions
    /// </summary>    
    public class ChunkManager : MonoBehaviour
    {
        public static ChunkManager Instance { get; private set; }

        [Header("Chunk settings")]
        public GameObject chunkParent;
        public float chunkSize = 50f;
        public int renderDistance = 1;  // radius of visible chunks beyond current chunk 
        public int chunkDeleteDistance = 5; // how far away until chunks get deleted for performance sake
        public int initialChunkRadius = 2;  // radius of chunks initially generated beyond current chunk
        public Vector3 initialPosition = Vector3.zero;

        private Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
        private Vector3Int currentPlayerChunk;

        // ------------------------------------
        [Space(6)]
        [Header("(Debug) Chunk visualization")]
        public bool visualizeChunks = true;
        public Color chunkBorderColor = Color.yellow;
        public float borderWidth = 0.1f;
        // ------------------------------------

        private void Awake() 
        {
            if (Instance == null)
            {
                Instance = this;
                // DontDestroyOnLoad(gameObject);
            }
            else { Destroy(gameObject); }

            if (chunkParent == null) { Debug.LogWarning("No chunks folder set!"); }
        }

        private void Start() {
            GenerateInitialChunks();
        }

        /// <summary>
        /// Generate initial chunks based on some initial size <br/>
        /// Initial size set in ChunkManager.cs
        /// </summary>
        private void GenerateInitialChunks()
        {
            Vector3Int centerChunkPos = GetChunkPosition(initialPosition);

            for (int x = -initialChunkRadius; x  <= initialChunkRadius; x++)
            {
                for (int y = -initialChunkRadius; y <= initialChunkRadius; y++)
                {
                    for (int z = -initialChunkRadius; z <= initialChunkRadius; z++)
                    {
                        Vector3Int chunkPos = centerChunkPos + new Vector3Int(x, y, z);
                        if (!chunks.ContainsKey(chunkPos))
                        {
                            GenerateChunk(chunkPos, true);
                        }
                    }
                }
            }

            currentPlayerChunk = centerChunkPos;
        }

        /// <summary>
        /// Helper function: what chunk does this position belong to
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns>Vector3Int chunk coordinate</returns>
        public Vector3Int GetChunkPosition(Vector3 worldPosition)
        {
            return new Vector3Int(
                Mathf.FloorToInt(worldPosition.x / chunkSize),
                Mathf.FloorToInt(worldPosition.y / chunkSize),
                Mathf.FloorToInt(worldPosition.z / chunkSize)
            );
        }

        /// <summary>
        /// Keep track of the chunk the player is in right now. <br/>
        /// If current chunk changes, update active chunks
        /// </summary>
        /// <param name="playerPosition"></param>
        public void UpdatePlayerPosition(Vector3 playerPosition)
        {
            Vector3Int newPlayerChunk = GetChunkPosition(playerPosition);
            if (newPlayerChunk != currentPlayerChunk)
            {
                currentPlayerChunk = newPlayerChunk;
                UpdateChunks();
            }
        }

        /// <summary>
        /// Main function for activating/deactivating, determining chunks to generate, and deleting chunks <br/>
        /// - one parameter controls render distance <br/>
        /// - one controls how far chunks are saved <br/>
        /// </summary>
        public void UpdateChunks()
        {
            HashSet<Vector3Int> chunksToKeep = new HashSet<Vector3Int>();

            // active/generate chunks within render distance
            for (int x = -renderDistance; x  <= renderDistance; x++)
            {
                for (int y = -renderDistance; y <= renderDistance; y++)
                {
                    for (int z = -renderDistance; z <= renderDistance; z++)
                    {
                        Vector3Int chunkPos = currentPlayerChunk + new Vector3Int(x, y, z);
                        chunksToKeep.Add(chunkPos);

                        if (!chunks.ContainsKey(chunkPos))
                        {
                            GenerateChunk(chunkPos);
                        }
                        else
                        {
                            chunks[chunkPos].SetActive(true);
                        }
                    }
                }
            }

            // deactivate out of range chunks
            List<Vector3Int> chunksToRemove = new List<Vector3Int>();
            foreach (var chunk in chunks)   // loop through dictionary pairs (V3I -> Chunk)
            {
                // get distance to current chunk
                Vector3Int distance = chunk.Key - currentPlayerChunk;
                // keeping chunks within a cube-like area, not a sphere, hence this LOC
                int maxAxisDistance = Mathf.Max(Mathf.Abs(distance.x), Mathf.Abs(distance.y), Mathf.Abs(distance.z));

                if (maxAxisDistance > chunkDeleteDistance)
                {
                    // deactivate + remove chunk if too far
                    chunk.Value.SetActive(false);
                    chunksToRemove.Add(chunk.Key);
                }
                else if (!chunksToKeep.Contains(chunk.Key))
                {
                    // not a chunk close enough, deactivate
                    chunk.Value.SetActive(false);
                }
            }

            // remove chunks too far
            foreach (var chunkPos in chunksToRemove)
            {
                Destroy(chunks[chunkPos].ChunkObject);
                chunks.Remove(chunkPos);
                // Debug.Log("Removing chunk!");
            }
        }

        /// <summary>
        /// High level chunk generation function, calls: <br/>
        /// - ParticleSpawner.GenerateObjectsForChunks, actual generation <br/>
        /// - InstantiateParticles(), actual instantiation of GameObjects <br/>
        /// - (debug) chunk border visualization
        /// </summary>
        /// <param name="chunkPos">chunk coordinate of new chunk</param>
        private void GenerateChunk(Vector3Int chunkPos, bool awake=false)
        {
            Chunk newChunk = new Chunk(chunkPos, chunkParent);
            chunks[chunkPos] = newChunk;    // chunks: V3I -> Chunk

            // generate coordinates for objects first
            List<Vector3> particlesPos = ParticleSpawner.Instance.GenerateObjectsForChunk(chunkPos, chunkSize);

            // actually instantiating them in Unity
            InstantiateParticles(newChunk, particlesPos);

            // initialize particles if GW active
            if (!awake && GWManager.Instance.IsWaveActive)
            {
                GWManager.Instance.InitializeChunk(newChunk);
            }

            // visualize chunk borders
            VisualizeChunkBorders(newChunk);
        }

        /// <summary>
        /// Actual instantiation of GameObjects within a chunk
        /// </summary>
        /// <param name="chunk">Chunk object</param>
        /// <param name="particlesPos">list of Vector3 positions</param>
        private void InstantiateParticles(Chunk chunk, List<Vector3> particlesPos)
        {
            List<GameObject> objects = new List<GameObject>();

            foreach (var particlePos in particlesPos)
            {
                GameObject particle = Instantiate(ParticleSpawner.Instance.particlePrefab, particlePos, 
                                                  Quaternion.identity, chunk.ChunkObject.transform);
                objects.Add(particle);
            }

            chunk.SetObjects(objects);
        }

        public IEnumerable<Chunk> GetActiveChunks()
        {
            return chunks.Values.Where(c => c.IsActive);
        }

        /// <summary>
        /// (debug) Generates border lines for a chunk
        /// </summary>
        /// <param name="chunk">Chunk object</param>
        private void VisualizeChunkBorders(Chunk chunk)
        {
            if (!visualizeChunks) return;

            Vector3 chunkOrigin = new Vector3(chunk.Position.x * chunkSize, chunk.Position.y * chunkSize, chunk.Position.z * chunkSize);
            
            GameObject borderContainer = new GameObject("ChunkBorder");
            borderContainer.transform.parent = chunk.ChunkObject.transform;
            borderContainer.transform.localPosition = Vector3.zero;

            for (int i = 0; i < 12; i++)
            {
                GameObject line = new GameObject($"Border_{i}");
                line.transform.parent = borderContainer.transform;
                LineRenderer lr = line.AddComponent<LineRenderer>();
                lr.useWorldSpace = true;
                lr.startWidth = borderWidth;
                lr.endWidth = borderWidth;
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startColor = chunkBorderColor;
                lr.endColor = chunkBorderColor;
                lr.positionCount = 2;

                Vector3 start, end;
                switch (i)
                {
                    // Bottom square
                    case 0: start = chunkOrigin; end = chunkOrigin + new Vector3(chunkSize, 0, 0); break;
                    case 1: start = chunkOrigin + new Vector3(chunkSize, 0, 0); end = chunkOrigin + new Vector3(chunkSize, 0, chunkSize); break;
                    case 2: start = chunkOrigin + new Vector3(chunkSize, 0, chunkSize); end = chunkOrigin + new Vector3(0, 0, chunkSize); break;
                    case 3: start = chunkOrigin + new Vector3(0, 0, chunkSize); end = chunkOrigin; break;
                    
                    // Top square
                    case 4: start = chunkOrigin + new Vector3(0, chunkSize, 0); end = chunkOrigin + new Vector3(chunkSize, chunkSize, 0); break;
                    case 5: start = chunkOrigin + new Vector3(chunkSize, chunkSize, 0); end = chunkOrigin + new Vector3(chunkSize, chunkSize, chunkSize); break;
                    case 6: start = chunkOrigin + new Vector3(chunkSize, chunkSize, chunkSize); end = chunkOrigin + new Vector3(0, chunkSize, chunkSize); break;
                    case 7: start = chunkOrigin + new Vector3(0, chunkSize, chunkSize); end = chunkOrigin + new Vector3(0, chunkSize, 0); break;
                    
                    // Vertical lines
                    case 8: start = chunkOrigin; end = chunkOrigin + new Vector3(0, chunkSize, 0); break;
                    case 9: start = chunkOrigin + new Vector3(chunkSize, 0, 0); end = chunkOrigin + new Vector3(chunkSize, chunkSize, 0); break;
                    case 10: start = chunkOrigin + new Vector3(chunkSize, 0, chunkSize); end = chunkOrigin + new Vector3(chunkSize, chunkSize, chunkSize); break;
                    case 11: start = chunkOrigin + new Vector3(0, 0, chunkSize); end = chunkOrigin + new Vector3(0, chunkSize, chunkSize); break;
                    
                    default: start = end = Vector3.zero; break;
                }

                lr.SetPosition(0, start);
                lr.SetPosition(1, end);
            }
        }

    }
} 