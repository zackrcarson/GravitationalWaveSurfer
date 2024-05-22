using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    // Config Parameters
    [Header("Paticle Prefabs")]
    [SerializeField] GameObject protonPrefab = null;
    [SerializeField] GameObject neutronPrefab = null;
    [SerializeField] GameObject electronPrefab = null;
    [SerializeField] GameObject antiProtonPrefab = null;
    [SerializeField] GameObject antiNeutronPrefab = null;
    [SerializeField] GameObject antiElectronPrefab = null;

    [SerializeField] public GameObject particleClumpPrefab = null;
    [SerializeField] public GameObject antiParticleClumpPrefab = null;
    
    [Header("Paticle Probability Distributions")]
    [SerializeField] float[] protonProbabilities = { 33.33f, 26.33f, 20f, 16.66f };
    [SerializeField] float[] neutronProbabilities = { 33.33f, 26.33f, 20f, 16.66f };
    [SerializeField] float[] electronProbabilities = { 33.34f, 26.33f, 20f, 16.66f };
    [SerializeField] float[] antiProtonProbabilities = { 0f, 7f, 13.33f, 16.66f };
    [SerializeField] float[] antiNeutronProbabilities = { 0f, 7f, 13.33f, 16.66f };

    [Header("Spawn Settings")]
    [SerializeField] public bool allowSpawning = true;
    [SerializeField] float randomSpawnTimeMin = .1f;
    [SerializeField] float randomSpawnTimeMax = 3f;

    [SerializeField] float playerBuffer = 1f;
    [SerializeField] float boundaryBuffer = 0.1f;

    [SerializeField] LayerMask doNotOverlap;

    [Header("Audio")]
    [SerializeField] string particleSpawnClipName = "particlePop";
    [SerializeField] float particleSpawnSpatialBlend = 1.0f;
    [SerializeField] float particleSpawnVolume = 0.7f;

    // Cached References
    float xMin, xMax, yMin, yMax;
    Player player = null;
    AudioManager audioManager = null;

    float protonProbability = 20f;
    float neutronProbability = 20f;
    float electronProbability = 20f;
    float antiProtonProbability = 13.33f;
    float antiNeutronProbability = 13.33f;

    [HideInInspector] public float particleAngularDrag = 0.001f;
    [HideInInspector] public float particleGravityScale = 0f;
    [HideInInspector] public CollisionDetectionMode2D particleCollisionDetectionMode;

    // State Variables
    float timer = 0f;
    bool canSpawn = true;
    bool hasStarted = false;
    bool firstParticle = true;

    // Start is called before the first frame update
    public void ExternalStart()
    {
        int difficulty = FindObjectOfType<GameManager>().difficulty;
        protonProbability = protonProbabilities[difficulty];
        neutronProbability = neutronProbabilities[difficulty];
        electronProbability = electronProbabilities[difficulty];
        antiProtonProbability = antiProtonProbabilities[difficulty];
        antiNeutronProbability = antiNeutronProbabilities[difficulty];

        player = FindObjectOfType<Player>();
        audioManager = FindObjectOfType<AudioManager>();

        SetupMoveBoundaries();

        ResetTimer();

        hasStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (allowSpawning)
        {
            if (hasStarted)
            {
                if (canSpawn)
                {
                    timer -= Time.deltaTime;

                    if (timer <= 0)
                    {
                        SpawnRandomParticle();

                        ResetTimer();
                    }
                }
            }
        }
    }

    public void AllowSpawning(bool isAllowed)
    {
        canSpawn = isAllowed;
    }

    private void SpawnRandomParticle()
    {
        GameObject particlePrefab = PickSpawnParticle();
        Vector3 spawnPosition = PickSpawnPosition(particlePrefab);

        if (!canSpawn)
        {
            return;
        }
        
        audioManager.PlayEffectAtLocation(spawnPosition, particleSpawnSpatialBlend, particleSpawnVolume, particleSpawnClipName);
        Instantiate(particlePrefab, spawnPosition, Quaternion.identity, transform);

        if (firstParticle)
        {
            if (particlePrefab.GetComponent<Particle>() != null)
            {
                Rigidbody2D thisRigidBody = particlePrefab.GetComponent<Particle>().GetComponent<Rigidbody2D>();

                particleAngularDrag = thisRigidBody.angularDrag;
                particleGravityScale = thisRigidBody.gravityScale;
                particleCollisionDetectionMode = thisRigidBody.collisionDetectionMode;
            }
            else if (particlePrefab.GetComponent<AntiParticle>() != null)
            {
                Rigidbody2D thisRigidBody = particlePrefab.GetComponent<AntiParticle>().GetComponent<Rigidbody2D>();

                particleAngularDrag = thisRigidBody.angularDrag;
                particleGravityScale = thisRigidBody.gravityScale;
                particleCollisionDetectionMode = thisRigidBody.collisionDetectionMode;
            }

            firstParticle = false;
        }
    }

    private GameObject PickSpawnParticle()
    {
        float pickParticle = Random.Range(0f, 100f);

        if (pickParticle <= protonProbability)
        {
            return protonPrefab;
        }
        else if (pickParticle > protonProbability && pickParticle <= (protonProbability + neutronProbability))
        {
            return neutronPrefab;
        }
        else if (pickParticle > neutronProbability && pickParticle <= (protonProbability + neutronProbability + electronProbability))
        {
            return electronPrefab;
        }
        else if (pickParticle > electronProbability && pickParticle <= (protonProbability + neutronProbability + electronProbability + antiProtonProbability))
        {
            return antiProtonPrefab;
        }
        else if (pickParticle > antiProtonProbability && pickParticle <= (protonProbability + neutronProbability + electronProbability + antiProtonProbability + antiNeutronProbability))
        {
            return antiNeutronPrefab;
        }
        else
        {
            return antiElectronPrefab;
        }
    }

    private Vector3 PickSpawnPosition(GameObject particlePrefab)
    {
        Random.InitState((int)System.DateTime.Now.Millisecond - 5);
        Vector3 spawnPosition = RandomPosition();

        while (!IsValidSpawnLocation(particlePrefab, spawnPosition))
        {
            if (!canSpawn)
            {
                break;
            }

            spawnPosition = RandomPosition();
        }

        return spawnPosition;
    }

    private Vector3 RandomPosition()
    {
        float xPosition = Random.Range(xMin, xMax);
        float yPosition = Random.Range(yMin, yMax);

        Vector3 spawnPosition = new Vector3(xPosition, yPosition, 0f);

        return spawnPosition;
    }

    private bool IsValidSpawnLocation(GameObject go, Vector3 position)
    {
        CircleCollider2D objectCollider = go.GetComponent<CircleCollider2D>();

        bool validPosition = false;

        float radius = objectCollider.radius * go.transform.localScale.x;
        Vector3 min = position - new Vector3(radius, radius, 0f);
        Vector3 max = position + new Vector3(radius, radius, 0f);

        Collider2D[] overlapObjects = Physics2D.OverlapAreaAll(min, max, doNotOverlap);

        validPosition = (overlapObjects.Length == 0);

        if (player == null) { player = FindObjectOfType<Player>(); }

        if (player != null) 
        {
            validPosition = (Vector3.Distance(position, player.transform.position) > playerBuffer);
        }

        return validPosition;
    }

    private void ResetTimer()
    {
        timer = Random.Range(randomSpawnTimeMin, randomSpawnTimeMax);
    }

    private void SetupMoveBoundaries()
    {
        Camera gameCamera = Camera.main;

        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + boundaryBuffer;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - boundaryBuffer;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + boundaryBuffer;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - boundaryBuffer;
    }
}
