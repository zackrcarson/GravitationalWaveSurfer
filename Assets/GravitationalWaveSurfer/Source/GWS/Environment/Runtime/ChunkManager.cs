using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

using GWS.Data;

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
        public float chunkSize = 500f;
        public int renderDistance = 1;  // radius of visible chunks beyond current chunk 
        public int chunkDeleteDistance = 4; // how far away until chunks get deleted for performance sake
        public Vector3 initialPosition = Vector3.zero;

        private Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
        private Vector3Int currentPlayerChunk;

        [Space(6)]
        [Header("Chunk randomization parameters")]
        public float minParticlePercentage = 0.5f;
        public float POIProbability = 0.05f;
        public float blackHoleProbability = 0.01f;
        public GameObject blackHole;
        public List<GameObject> POIList;
        public int POILayer;

        [Space(6)]
        [Header("POI properties' randomization parameters")]
        public int minOneTimeValue = 100;
        public int maxOneTimeValue = 1000;
        public int minPassiveValue = 1;
        public int maxPassiveValue = 5;

        // ------------------------------------
        [Space(6)]
        [Header("(Debug) Chunk visualization")]
        public bool visualizeChunks = true;
        public Color chunkBorderColor = Color.yellow;
        public float borderWidth = 0.1f;
        // ------------------------------------

        [Space(6)]
        [Header("Particle spawn batching")]
        [Tooltip("Max number of particle GameObjects spawned per frame. Keeps a burst of newly " +
                 "generated chunks from instantiating thousands of objects in a single frame.")]
        public int particleSpawnBudgetPerFrame = 200;

        // pooled particles, reused instead of Destroy()/Instantiate() on every chunk load/unload
        private readonly Stack<GameObject> particlePool = new Stack<GameObject>();
        private readonly Queue<(Chunk chunk, Vector3 position)> pendingParticleSpawns = new Queue<(Chunk, Vector3)>();
        private readonly Dictionary<Chunk, int> pendingParticleCountForChunk = new Dictionary<Chunk, int>();
        private Coroutine particleSpawnCoroutine;
        private Transform particlePoolHolder;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // DontDestroyOnLoad(gameObject);

                particlePoolHolder = new GameObject("ParticlePool (pooled, inactive)").transform;
                particlePoolHolder.SetParent(transform);
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

            for (int x = -renderDistance; x  <= renderDistance; x++)
            {
                for (int y = -renderDistance; y <= renderDistance; y++)
                {
                    for (int z = -renderDistance; z <= renderDistance; z++)
                    {
                        Vector3Int chunkPos = centerChunkPos + new Vector3Int(x, y, z);
                        if (!chunks.ContainsKey(chunkPos))
                        {
                            GenerateChunk(chunkPos);
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
                ReclaimChunk(chunks[chunkPos]);
                chunks.Remove(chunkPos);
                // Debug.Log("Removing chunk!");
            }
        }

        /// <summary>
        /// Returns a chunk's particles to <see cref="particlePool"/> instead of letting them be
        /// destroyed with the chunk, so revisiting the area later reuses them instead of paying
        /// for a fresh Instantiate() burst.
        /// </summary>
        /// <param name="chunk">Chunk about to be removed.</param>
        private void ReclaimChunk(Chunk chunk)
        {
            if (!chunk.HasPOI && !chunk.HasBlackHole)
            {
                foreach (var particle in chunk.Objects)
                {
                    if (particle == null) continue;
                    particle.SetActive(false);
                    particle.transform.SetParent(particlePoolHolder);
                    particlePool.Push(particle);
                }
            }

            Destroy(chunk.ChunkObject);
        }

        /// <summary>
        /// High level chunk generation function, calls: <br/>
        /// - ParticleSpawner.GenerateObjectsForChunks, actual generation <br/>
        /// - InstantiateParticles(), actual instantiation of GameObjects <br/>
        /// - InstantiateObject(), for POIs and black holes <br/>
        /// - (debug) chunk border visualization
        /// </summary>
        /// <param name="chunkPos">chunk coordinate of new chunk</param>
        private void GenerateChunk(Vector3Int chunkPos)
        {
            if (!GWManager.Instance) return; 
                
            Chunk newChunk = new Chunk(chunkPos, chunkParent);
            chunks[chunkPos] = newChunk;    // chunks: V3I -> Chunk
            newChunk.SetActive(true);

            // random chunk attributes and parameters
            float density = UnityEngine.Random.Range(minParticlePercentage, 1f);
            float POI = UnityEngine.Random.Range(0f, 1f);

            /*
            approximate probability distribution:
            |    POI    | BLACK HOLE |                        PARTICLES                        |
            determined by parameters
            */
            
            // restrict POI generation if any adjacent chunk already has a POI
            bool hasPOINearby = false;
            if (POI <= (POIProbability + blackHoleProbability))
            {
                hasPOINearby = CheckMassiveObjectsNearby(chunkPos);
            }

            if (POI > (POIProbability + blackHoleProbability) || hasPOINearby)
            {
                // generate coordinates for particles first
                List<Vector3> particlesPos = ParticleSpawner.Instance.GenerateParticlesForChunk(chunkPos, chunkSize, density);
                // actually instantiating them in Unity, spread across frames (see ProcessParticleSpawnQueue).
                // GWManager picks up newly-populated chunks itself once Chunk.IsFullyPopulated is true.
                InstantiateParticles(newChunk, particlesPos);
            }
            else if (POI < POIProbability)
            {
                Debug.Log("Generating POI");
                // generate POI for chunk
                Vector3 chunkCenter = new Vector3(chunkPos.x * chunkSize + chunkSize / 2,
                                    chunkPos.y * chunkSize + chunkSize / 2,
                                    chunkPos.z * chunkSize + chunkSize / 2);

                chunkCenter += new Vector3(UnityEngine.Random.Range(0f, chunkSize * 0.3f), 
                                           UnityEngine.Random.Range(0f, chunkSize * 0.3f), 
                                           UnityEngine.Random.Range(0f, chunkSize * 0.3f));

                // pick random POI and instantiate the GameObject
                int POIIndex = UnityEngine.Random.Range(0, POIList.Count);
                InstantiateObject(newChunk, chunkCenter, POIList[POIIndex], "POI");
                newChunk.SetPOI(true);
            }
            else
            {
                Debug.Log("Generating black hole");
                // generate black hole
                Vector3 chunkCenter = new Vector3(chunkPos.x * chunkSize + chunkSize / 2,
                                    chunkPos.y * chunkSize + chunkSize / 2,
                                    chunkPos.z * chunkSize + chunkSize / 2);

                chunkCenter += new Vector3(UnityEngine.Random.Range(0f, chunkSize * 0.1f), 
                                           UnityEngine.Random.Range(0f, chunkSize * 0.1f), 
                                           UnityEngine.Random.Range(0f, chunkSize * 0.1f));

                InstantiateObject(newChunk, chunkCenter, blackHole, "Black Hole");
                newChunk.SetBlackHole(true);
            }

            // visualize chunk borders
            VisualizeChunkBorders(newChunk);
        }

        /// <summary>
        /// Helper function: checks if POI / black holes exists in any adjacent Chunk (6 directions not diagonals)
        /// </summary>
        /// <param name="chunkPos">current chunk coordinate</param>
        /// <returns>boolean, true if exists, false otherwise</returns>
        private bool CheckMassiveObjectsNearby(Vector3Int chunkPos)
        {
            Vector3Int[] adjacentPositions = new Vector3Int[]
            {
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, -1, 0),
                new Vector3Int(0, 0, 1),
                new Vector3Int(0, 0, -1)
            };

            foreach (Vector3Int offset in adjacentPositions)
            {
                Vector3Int adjacentPos = chunkPos + offset;
                if (chunks.TryGetValue(adjacentPos, out Chunk adjacentChunk))
                {
                    if (adjacentChunk.HasPOI || adjacentChunk.HasBlackHole) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Queues a chunk's particles for instantiation, spread across frames by
        /// <see cref="ProcessParticleSpawnQueue"/> instead of instantiating them all at once.
        /// </summary>
        /// <param name="chunk">Chunk object</param>
        /// <param name="particlesPos">list of Vector3 positions</param>
        private void InstantiateParticles(Chunk chunk, List<Vector3> particlesPos)
        {
            if (particlesPos.Count == 0)
            {
                chunk.SetFullyPopulated(true);
                return;
            }

            chunk.SetFullyPopulated(false);
            pendingParticleCountForChunk[chunk] = particlesPos.Count;

            foreach (var particlePos in particlesPos)
            {
                pendingParticleSpawns.Enqueue((chunk, particlePos));
            }

            if (particleSpawnCoroutine == null)
            {
                particleSpawnCoroutine = StartCoroutine(ProcessParticleSpawnQueue());
            }
        }

        /// <summary>
        /// Spawns queued particles a limited number at a time (<see cref="particleSpawnBudgetPerFrame"/>)
        /// so that a burst of newly generated chunks doesn't instantiate thousands of GameObjects
        /// within a single frame.
        /// </summary>
        private IEnumerator ProcessParticleSpawnQueue()
        {
            while (pendingParticleSpawns.Count > 0)
            {
                for (int i = 0; i < particleSpawnBudgetPerFrame && pendingParticleSpawns.Count > 0; i++)
                {
                    var (chunk, position) = pendingParticleSpawns.Dequeue();
                    SpawnQueuedParticle(chunk, position);
                }

                yield return null;
            }

            particleSpawnCoroutine = null;
        }

        /// <summary>
        /// Rents (or instantiates) a single particle for a chunk and, once that chunk's whole
        /// batch has spawned, marks it fully populated.
        /// </summary>
        private void SpawnQueuedParticle(Chunk chunk, Vector3 position)
        {
            // chunk may have been reclaimed/destroyed before its particles reached the front of the queue
            if (chunk.ChunkObject != null)
            {
                GameObject particle = RentParticle();
                particle.transform.SetPositionAndRotation(position, Quaternion.identity);
                particle.transform.SetParent(chunk.ChunkObject.transform);
                particle.SetActive(true);
                chunk.AddObject(particle);
            }

            if (--pendingParticleCountForChunk[chunk] <= 0)
            {
                pendingParticleCountForChunk.Remove(chunk);
                chunk.SetFullyPopulated(true);
            }
        }

        /// <summary>
        /// Returns a pooled particle if one is available, otherwise instantiates a new one.
        /// </summary>
        private GameObject RentParticle()
        {
            while (particlePool.Count > 0)
            {
                GameObject pooled = particlePool.Pop();
                if (pooled != null) return pooled;
            }

            return Instantiate(ParticleSpawner.Instance.particlePrefab);
        }

        /// <summary>
        /// Instantiate object at some position in some chunk
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="chunkCenter"></param>
        /// <param name="POI"></param>
        private void InstantiateObject(Chunk chunk, Vector3 chunkCenter, GameObject gameObject, string tag ="Object")
        {
            List<GameObject> objects = new List<GameObject>();
            GameObject POIGameObject = Instantiate(gameObject, chunkCenter, Quaternion.identity, chunk.ChunkObject.transform);
            
            POIGameObject.tag = tag;
            POIGameObject.layer = POILayer;

            if (tag == "POI")
            {
                POIGameObject.transform.localScale *= chunkSize * 0.08f;

                POIData poiData = POIGameObject.GetComponent<POIData>();
                poiData.Initialize(GetRandomPassiveValue(),
                                   GetRandomOneTimeValue(),
                                   POIManager.Instance.questionDatabase.GetRandomQuestion());
            }

            objects.Add(POIGameObject);
            chunk.SetObjects(objects);
        }

        /// <summary>
        /// Helper function: return an iterable of Chunk that are active right now
        /// </summary>
        /// <returns>IEnumerable of Chunk</returns>
        public IEnumerable<Chunk> GetActiveChunks()
        {
            return chunks.Values.Where(c => c.IsActive);
        }

        /// <summary>
        /// Helper function: return the Chunk object where the player is currently in
        /// </summary>
        /// <returns></returns>
        public Chunk GetCurrentChunk()
        {
            return chunks[currentPlayerChunk];
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

        /// <summary>
        /// Helper function: returns random integer within possible range for <br/>
        /// passive hydrogen collection increase
        /// </summary>
        /// <returns></returns>
        public int GetRandomPassiveValue()
        {
            return UnityEngine.Random.Range(minPassiveValue, maxPassiveValue);
        }

        /// <summary>
        /// Helper function: returns random integer within possible range for <br/>
        /// one time hydrogen bonus
        /// </summary>
        /// <returns></returns>
        public int GetRandomOneTimeValue()
        {
            return UnityEngine.Random.Range(minOneTimeValue, maxOneTimeValue);
        }

    }
} 