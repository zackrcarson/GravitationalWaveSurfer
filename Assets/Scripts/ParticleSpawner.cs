using System.Collections;
using System.Collections.Generic;
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

    [Header("Paticle Probability Distribution")]
    [SerializeField] float protonProbability = 20f;
    [SerializeField] float neutronProbability = 20f;
    [SerializeField] float electronProbability = 20f;
    [SerializeField] float antiProtonProbability = 13.33f;
    [SerializeField] float antiNeutronProbability = 13.33f;

    [Header("Spawn Settings")]
    [SerializeField] float randomSpawnTimeMin = .1f;
    [SerializeField] float randomSpawnTimeMax = 3f;

    [SerializeField] float playerBuffer = 1f;
    [SerializeField] float boundaryBuffer = 0.1f;

    [SerializeField] LayerMask doNotOverlap;

    // Cached References
    float xMin, xMax, yMin, yMax;
    Player player = null;

    // State Variables
    float timer = 0f;
    bool canSpawn = true;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();

        SetupMoveBoundaries();

        ResetTimer();
    }

    // Update is called once per frame
    void Update()
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

        Instantiate(particlePrefab, spawnPosition, Quaternion.identity, transform);
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
        if (player == null) { canSpawn = false; return false; }

        validPosition = (Vector3.Distance(position, player.transform.position) > playerBuffer);

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
