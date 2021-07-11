using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float playerSpeed = 1f;
    [SerializeField] float initialRandomTorque = 0.001f;
    [SerializeField] float initialRandomPush = 0.4f;
    [SerializeField] float xBoundaryPadding = 0.1f;
    [SerializeField] float yBoundaryPadding = 0.1f;

    // Cached References
    Rigidbody2D rigidBody = null;
    GameOver gameOver = null;
    MicroBlackHole microBlackHole = null;
    new ConstantForce2D constantForce = null;

    float xMin, xMax, yMin, yMax;

    bool canMove = true;
    bool isMoving = false;
    bool hasStartedControlling = false;

    // State Variables
    public List<GameObject> particles = null;
    public List<string> particleNames = null;
    Vector2 thisToBlackHole;

    // Constants
    const string ANTI_PREFIX = "Anti-";
    const string PROTON_NAME = "Proton";
    const string NEUTRON_NAME = "Neutron";
    const string ELECTRON_NAME = "Electron";

    // Start is called before the first frame update
    void Start()
    {
        particles = new List<GameObject>();
        particleNames = new List<string>();

        gameOver = FindObjectOfType<GameOver>();
        rigidBody = GetComponent<Rigidbody2D>();
        microBlackHole = FindObjectOfType<MicroBlackHole>();
        constantForce = GetComponent<ConstantForce2D>();
        constantForce.enabled = false;

        SetupMoveBoundaries();
        
        RandomKick();
    }

    public void AllowMovement(bool isAllowed)
    {
        canMove = isAllowed;
    }

    private void RandomKick()
    {
        float randomRotation = initialRandomTorque * Random.Range(30f, 60f);
        rigidBody.AddTorque(randomRotation, ForceMode2D.Impulse);

        Vector2 randomPush = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rigidBody.velocity = initialRandomPush * randomPush;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            gameOver.StartGameOver("Black Hole");
        }

        if (canMove)
        {
            Move();
        }

        MicroBlackHole();
    }

    private void MicroBlackHole()
    {
        if (microBlackHole.isActive)
        {
            thisToBlackHole = transform.position - microBlackHole.transform.position;

            if (thisToBlackHole.magnitude < microBlackHole.interactionRadius)
            {
                if (thisToBlackHole.magnitude < microBlackHole.eventHorizon)
                {
                    constantForce.force = microBlackHole.eventHorizonForce * thisToBlackHole.normalized;
                    canMove = false;
                }
                else
                {
                    constantForce.force = microBlackHole.force * thisToBlackHole.normalized;
                    canMove = true;
                }

                constantForce.force = microBlackHole.force * thisToBlackHole.normalized;

                constantForce.enabled = true;
            }
            else
            {
                constantForce.enabled = false;
                canMove = true;
            }
        }
        else
        {
            constantForce.enabled = false;
        }
    }

    private void Move()
    {
        float currentInputX = Input.GetAxis("Horizontal");
        float currentInputY = Input.GetAxis("Vertical");

        if (currentInputX != 0 || currentInputY != 0)
        {
            hasStartedControlling = true;
        }

        if (hasStartedControlling)
        {
            float currentVelocity = playerSpeed;

            Vector2 newVelocity = new Vector2(currentVelocity * currentInputX, currentVelocity * currentInputY);

            rigidBody.velocity = newVelocity;

            isMoving = (Mathf.Abs(rigidBody.velocity.x) > Mathf.Epsilon) || (Mathf.Abs(rigidBody.velocity.y) > Mathf.Epsilon);
        }

        ClampPosition();
    }

    private void ClampPosition()
    {
        if (transform.position.x > xMax)
        {
            Vector3 newPos = new Vector3(xMax, transform.position.y, transform.position.z);
            transform.position = newPos;
        }

        if (transform.position.x < xMin)
        {
            Vector3 newPos = new Vector3(xMin, transform.position.y, transform.position.z);
            transform.position = newPos;
        }

        if (transform.position.y > yMax)
        {
            Vector3 newPos = new Vector3(transform.position.x, yMax, transform.position.z);
            transform.position = newPos;
        }

        if (transform.position.y < yMin)
        {
            Vector3 newPos = new Vector3(transform.position.x, yMin, transform.position.z);
            transform.position = newPos;
        }
    }

    private void SetupMoveBoundaries()
    {
        Camera gameCamera = Camera.main;

        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + xBoundaryPadding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - xBoundaryPadding;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + yBoundaryPadding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - yBoundaryPadding;
    }

    public void KillPlayer(string victimName)
    {
        gameOver.StartGameOver(victimName);
        Destroy(gameObject);

        // TODO: Player Destroy Effect
    }

    public bool AnnihilateParticle(string antiParticleName)
    {
        if (particleNames.Contains(antiParticleName.Replace(ANTI_PREFIX, "")))
        {
            Debug.Log("looking");
            for (int i = particles.Count; i-- > 0;)
            {
                Debug.Log(particles[i]);
                if (particles[i].tag == antiParticleName.Replace(ANTI_PREFIX, ""))
                {
                    Debug.Log("Destroying");
                    GameObject particleToDelete = particles[i].gameObject;

                    particles.RemoveAt(i);
                    particleNames.RemoveAt(i);

                    GameManager.instance.RemoveParticle(particleToDelete.tag);
                    Destroy(particleToDelete);

                    // TODO: Particle Effect

                    return true;
                }
            }

            return false;
        }
        else if (antiParticleName == ANTI_PREFIX + PROTON_NAME || antiParticleName == ANTI_PREFIX + ELECTRON_NAME)
        {
            Debug.Log("killing??");
            KillPlayer(antiParticleName.Replace(ANTI_PREFIX, ""));

            return true;

            // TODO: Particle Effect
        }
        else
        {
            return false; // Do nothing - neutron can bounce off.
        }
    }

    public List<GameObject> AnnihilateParticles(List<GameObject> antiParticles, List<string> antiParticleNames)
    {
        if (particles.Count == 0)
        {
            if (antiParticleNames.Contains(ANTI_PREFIX + PROTON_NAME))
            {
                KillPlayer(PROTON_NAME);

                if (antiParticles.Count > 0)
                {
                    Destroy(antiParticles[0].transform.parent.gameObject);
                }
                antiParticles = new List<GameObject>();

                // TODO: Particle Effect
            }
            else if (antiParticleNames.Contains(ANTI_PREFIX + ELECTRON_NAME))
            {
                KillPlayer(ELECTRON_NAME);

                if (antiParticles.Count > 0)
                {
                    Destroy(antiParticles[0].transform.parent.gameObject);
                }
                antiParticles = new List<GameObject>();

                // TODO: Particle Effect
            }
            else if (antiParticleNames.Contains(ANTI_PREFIX + NEUTRON_NAME))
            {
                // Do nothing
            }
            else
            {
                Debug.LogError("Other condition found?");
            }
        }
        else if (antiParticleNames.Contains(ANTI_PREFIX + PROTON_NAME) && !particleNames.Contains(PROTON_NAME))
        {
            KillPlayer(PROTON_NAME);

            if (antiParticles.Count > 0)
            {
                Destroy(antiParticles[0].transform.parent.gameObject);
            }
            antiParticles = new List<GameObject>();

            // TODO: Particle Effect
        }
        else if (antiParticleNames.Contains(ANTI_PREFIX + ELECTRON_NAME) && !particleNames.Contains(ELECTRON_NAME))
        {
            KillPlayer(ELECTRON_NAME); 

            if (antiParticles.Count > 0)
            {
                Destroy(antiParticles[0].transform.parent.gameObject);
            }
            antiParticles = new List<GameObject>();

            // TODO: Particle Effect
        }
        else if (antiParticleNames.Contains(ANTI_PREFIX + NEUTRON_NAME) && !particleNames.Contains(NEUTRON_NAME) && !antiParticleNames.Contains(ANTI_PREFIX + ELECTRON_NAME) && !antiParticleNames.Contains(ANTI_PREFIX + PROTON_NAME))
        {
            // Do nothing
        }
        else
        {
            for (int i = antiParticles.Count; i-- > 0;)
            {
                if (particleNames.Contains(antiParticles[i].tag.Replace(ANTI_PREFIX, "")))
                {
                    for (int j = particles.Count; j-- > 0;)
                    {
                        if (ANTI_PREFIX + particles[j].tag == antiParticles[i].tag)
                        {
                            GameObject particleToDelete = particles[j];
                            particles.RemoveAt(j);
                            particleNames.RemoveAt(j);
                            Destroy(particleToDelete);

                            GameManager.instance.RemoveParticle(particleToDelete.tag);

                            GameObject antiParticleToDelete = antiParticles[i];
                            antiParticles.RemoveAt(i);
                            Destroy(antiParticleToDelete);

                            break;
                        }
                    }
                }
                else if (antiParticles[i].tag == ANTI_PREFIX + PROTON_NAME || antiParticles[i].tag == ANTI_PREFIX + ELECTRON_NAME)
                {
                    KillPlayer(antiParticles[i].tag.Replace(ANTI_PREFIX, ""));

                    if (antiParticles.Count > 0)
                    {
                        Destroy(antiParticles[0].transform.parent.gameObject);
                    }
                    antiParticles = new List<GameObject>();

                    // TODO: Particle Effect
                }
                else
                {
                    // Do nothing - neutron can bounce off.
                }
            }
        }

        return antiParticles;
    }

    public void AddParticle(GameObject particle)
    {
        foreach (GameObject p in particles)
        {
            if (p == particle)
            {
                Debug.LogWarning("Match " + p + " found when adding particle " + particle + " to list! Ignoring. No need to focus on this warning, it is alright.");
                return;
            }
        }

        particles.Add(particle);
        particleNames.Add(particle.tag);

        GameManager.instance.AddParticle(particle.tag, true);
    }

    public void AddParticles(List<GameObject> newParticles, List<string> newParticleTypes)
    {
        for (int i = newParticles.Count; i-- > 0;)
        {
            if (particles.Contains(newParticles[i]))
            {
                Debug.LogWarning("Match found when adding particle " + newParticles[i] + " to list! Ignoring. No need to focus on this warning, it is alright.");

                newParticles.RemoveAt(i);
                newParticleTypes.RemoveAt(i);
            }
        }

        if (newParticles.Count > 0)
        {
            particles.AddRange(newParticles);
            particleNames.AddRange(newParticleTypes);

            GameManager.instance.AddParticles(newParticleTypes);
        }
    }
}
