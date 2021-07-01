using System.Collections.Generic;
using UnityEngine;

public class ParticleClump : MonoBehaviour
{
    // Cached References
    Rigidbody2D rigidBody = null;
    MicroBlackHole microBlackHole = null;
    new ConstantForce2D constantForce = null;
    Vector2 thisToBlackHole;

    // State Variables
    List<string> particles = null;
    public bool touchedFirst = false;
    Vector2 currentVelocity;
    float currentAngularVelocity, currentInertia;
    int currentMass;

    // Start is called before the first frame update
    public void NewClump()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        microBlackHole = FindObjectOfType<MicroBlackHole>();
        constantForce = GetComponent<ConstantForce2D>();
        constantForce.enabled = false;

        particles = new List<string>();
    }

    public void StoreCurrentAngularMomentum()
    {
        currentMass = particles.Count;
        currentVelocity = rigidBody.velocity;
        currentAngularVelocity = rigidBody.angularVelocity;
        currentInertia = rigidBody.inertia;
    }

    public void NewAngularMomentum(Vector2 otherVelocity, float otherAngularVelocity, float otherInertia, bool isNewClump, float newParticleRadius, int newNumParticles)
    {
        if (isNewClump)
        {
            rigidBody.velocity = otherVelocity;
            rigidBody.angularVelocity = otherAngularVelocity;
            rigidBody.inertia = otherInertia;
        }
        else
        {
            rigidBody.inertia += (otherInertia + (newNumParticles * newParticleRadius * newParticleRadius)); // Parallel axis theorem I_offset = I + md^2
            rigidBody.velocity = ((currentMass * currentVelocity) + (newNumParticles * otherVelocity)) / (newNumParticles + currentMass); // Conservation of linear momentum
            rigidBody.angularVelocity = ((currentInertia * currentAngularVelocity) + (otherInertia * otherAngularVelocity)) / rigidBody.inertia; // Conservation of angular momentum
        }
    }

    private void Update()
    {
        MicroBlackHole();
    }

    private void MicroBlackHole()
    {
        if (constantForce != null)
        {
            if (microBlackHole.isActive)
            {
                thisToBlackHole = transform.position - microBlackHole.transform.position;

                if (thisToBlackHole.magnitude < microBlackHole.interactionRadius)
                {
                    if (thisToBlackHole.magnitude < microBlackHole.eventHorizon)
                    {
                        constantForce.force = microBlackHole.eventHorizonForce * thisToBlackHole.normalized;
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
    }

    public void AddParticle(string particle)
    {
        particles.Add(particle);
    }

    private void OnCollisionEnter2D(Collision2D otherCollider)
    {
        if (otherCollider.gameObject.GetComponent<ParticleClump>())
        {
            if (!otherCollider.gameObject.GetComponent<ParticleClump>().touchedFirst)
            {
                touchedFirst = true;

                AddClumpToOtherClump(otherCollider.gameObject.GetComponent<ParticleClump>());
            }
        }
        else if (otherCollider.gameObject.GetComponent<AntiParticleClump>())
        {

        }
        else if (otherCollider.gameObject.GetComponent<Player>())
        {

        }
    }

    public void AddClumpToOtherClump(ParticleClump otherClump)
    {
        constantForce.enabled = false;
        Destroy(constantForce);

        Vector2 thisVelocity = rigidBody.velocity;
        float thisAngularVelocity = rigidBody.angularVelocity;
        float thisInertia = rigidBody.inertia;
        Vector2 thisDistance = transform.position - otherClump.transform.position;

        rigidBody.angularVelocity = 0f;
        rigidBody.velocity = Vector2.zero;
        Destroy(rigidBody);

        Destroy(GetComponent<WaveRider>());
        Destroy(GetComponent<FreeParticle>());

        otherClump.StoreCurrentAngularMomentum();
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
            children.Add(child);
        foreach (Transform child in children)
        {
            child.parent = otherClump.transform;
            otherClump.AddParticle(child.tag);
        }
        otherClump.NewAngularMomentum(thisVelocity, thisAngularVelocity, thisInertia, false, thisDistance.magnitude, particles.Count);

        Destroy(gameObject);
    }
}


/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleClump : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float initialRandomTorque = 0.001f;
    [SerializeField] float initialRandomPush = 0.4f;

    // Cached References
    Rigidbody2D rigidBody = null;
    MicroBlackHole microBlackHole = null;
    new ConstantForce2D constantForce = null;
    Vector2 thisToBlackHole;

    // State Variables
    List<string> particles = null;
    public bool touchedFirst = false;

    // Start is called before the first frame update
    public void NewClump()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        microBlackHole = FindObjectOfType<MicroBlackHole>();
        constantForce = GetComponent<ConstantForce2D>();
        constantForce.enabled = false;

        particles = new List<string>();
    }

    private void Update()
    {
        MicroBlackHole();
    }

    private void MicroBlackHole()
    {
        if (constantForce != null)
        {
            if (microBlackHole.isActive)
            {
                thisToBlackHole = transform.position - microBlackHole.transform.position;

                if (thisToBlackHole.magnitude < microBlackHole.interactionRadius)
                {
                    if (thisToBlackHole.magnitude < microBlackHole.eventHorizon)
                    {
                        constantForce.force = microBlackHole.eventHorizonForce * thisToBlackHole.normalized;
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
    }

    public void ExternalRandomKick()
    {
        StartCoroutine(RandomKick());
    }

    private IEnumerator RandomKick()
    {
        yield return null;

        rigidBody.angularVelocity = 0f;
        rigidBody.velocity = Vector2.zero;

        yield return null;

        float randomRotation = initialRandomTorque * Random.Range(30f, 60f);
        rigidBody.AddTorque(randomRotation, ForceMode2D.Impulse);

        Vector2 randomPush = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rigidBody.velocity = initialRandomPush * randomPush;
    }

    public void AddParticle(string particle, bool kick)
    {
        particles.Add(particle);

        rigidBody.angularVelocity = 0f;
        rigidBody.velocity = Vector2.zero;

        if (kick)
        {
            StartCoroutine(RandomKick());
        }
    }

    private void OnCollisionEnter2D(Collision2D otherCollider)
    {
        if (otherCollider.gameObject.GetComponent<ParticleClump>())
        {
            if (!otherCollider.gameObject.GetComponent<ParticleClump>().touchedFirst)
            {
                touchedFirst = true;

                AddClumpToOtherClump(otherCollider.gameObject.GetComponent<ParticleClump>());
            }
        }
        else if (otherCollider.gameObject.GetComponent<AntiParticleClump>())
        {

        }
        else if (otherCollider.gameObject.GetComponent<Player>() || otherCollider.gameObject.GetComponentInParent<Player>())
        {

        }
    }

    public void AddClumpToOtherClump(ParticleClump otherClump)
    {
        constantForce.enabled = false;
        Destroy(constantForce);

        rigidBody.angularVelocity = 0f;
        rigidBody.velocity = Vector2.zero;
        Destroy(rigidBody);

        Destroy(GetComponent<WaveRider>());
        Destroy(GetComponent<FreeParticle>());

        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
            children.Add(child);
        foreach (Transform child in children)
        {
            child.parent = otherClump.transform;
            otherClump.AddParticle(child.tag, false);
        }

        otherClump.ExternalRandomKick();

        Destroy(gameObject);
    }
}*/
