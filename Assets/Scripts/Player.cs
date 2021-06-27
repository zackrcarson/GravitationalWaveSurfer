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
    ConstantForce2D constantForce = null;

    float xMin, xMax, yMin, yMax;

    bool canMove = true;
    bool isMoving = false;
    bool hasStartedControlling = false;

    // State Variables
    List<Pickup> particles = null;
    List<string> particleNames = null;
    Vector2 thisToBlackHole;

    // Constants
    const string ANTI_PREFIX = "Anti-";
    const string PROTON_NAME = "Proton";
    const string NEUTRON_NAME = "Neutron";
    const string ELECTRON_NAME = "Electron";

    // Start is called before the first frame update
    void Start()
    {
        particles = new List<Pickup>();
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
                }

                constantForce.force = microBlackHole.force * thisToBlackHole.normalized;

                constantForce.enabled = true;
            }
            else
            {
                constantForce.enabled = false;
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

    private void FindAndRemoveParticle(string particleName)
    {
        int number = 0;
        GameObject particleToDelete = null;

        for (int i = particles.Count; i-- > 0;)
        {
            if (particles[i].tag == particleName)
            {
                number = i;
                particleToDelete = particles[i].gameObject;

                break;
            }
        }

        if (particleToDelete == null)
        {
            Debug.LogError("More debugging needed! No particle to delete found.");
        }
        else
        {
            particles.RemoveAt(number);
            particleNames.RemoveAt(number);

            GameManager.instance.RemoveParticle(particleToDelete.tag);
            Destroy(particleToDelete);
        }
    }

    public void KillPlayer(string victimName)
    {
        gameOver.StartGameOver(victimName);
        Destroy(gameObject);

        // TODO: Player Destroy Effect
    }

    public bool AnnihilateParticles(List<string> antiParticleNames)
    {
        if (particles.Count == 0)
        {
            if (antiParticleNames.Contains(ANTI_PREFIX + PROTON_NAME))
            {
                KillPlayer(PROTON_NAME);
                return true;
            }
            else if (antiParticleNames.Contains(ANTI_PREFIX + ELECTRON_NAME))
            {
                KillPlayer(ELECTRON_NAME);
                return true;
            }
            else if (antiParticleNames.Contains(ANTI_PREFIX + NEUTRON_NAME))
            {
                return false;
            }
            else
            {
                Debug.LogError("Other condition found?");
                return true;
            }
        }
        else if (antiParticleNames.Contains(ANTI_PREFIX + PROTON_NAME) && !particleNames.Contains(PROTON_NAME))
        {
            KillPlayer(PROTON_NAME);
            return true;
        }
        else if (antiParticleNames.Contains(ANTI_PREFIX + ELECTRON_NAME) && !particleNames.Contains(ELECTRON_NAME))
        {
            KillPlayer(ELECTRON_NAME);
            return true;
        }
        else if (antiParticleNames.Contains(ANTI_PREFIX + NEUTRON_NAME) && !particleNames.Contains(NEUTRON_NAME) && !antiParticleNames.Contains(ANTI_PREFIX + ELECTRON_NAME) && !antiParticleNames.Contains(ANTI_PREFIX + PROTON_NAME))
        {
            return false;
        }
        else
        {
            foreach (string anti in antiParticleNames)
            {
                string particle = anti.Replace(ANTI_PREFIX, "");

                FindAndRemoveParticle(particle);
            }

            return true;
        }
    }

    public void AddParticle(Pickup particle)
    {
        particles.Add(particle);
        particleNames.Add(particle.tag);
    }
}
