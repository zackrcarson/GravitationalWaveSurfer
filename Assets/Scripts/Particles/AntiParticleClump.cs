using System.Collections.Generic;
using UnityEngine;

public class AntiParticleClump : MonoBehaviour
{
    // Cached References
    Rigidbody2D rigidBody = null;
    MicroBlackHole microBlackHole = null;
    new ConstantForce2D constantForce = null;
    Vector2 thisToBlackHole;

    // State Variables
    public List<GameObject> antiParticles = null;
    public bool touchedFirst = false;
    Vector2 currentVelocity;
    float currentAngularVelocity, currentInertia;
    int currentMass;

    // Constants
    const string ANTI_PREFIX = "Anti-";

    // Start is called before the first frame update
    public void NewClump()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        microBlackHole = FindObjectOfType<MicroBlackHole>();
        constantForce = GetComponent<ConstantForce2D>();
        constantForce.enabled = false;

        antiParticles = new List<GameObject>();
    }

    public void StoreCurrentAngularMomentum()
    {
        currentMass = antiParticles.Count;
        currentVelocity = rigidBody.velocity;
        currentAngularVelocity = rigidBody.angularVelocity;
        currentInertia = rigidBody.inertia;
    }

    public void NewAngularMomentum(Vector2 otherVelocity, float otherAngularVelocity, float otherInertia, bool isNewClump, float newParticleRadius, int newNumAntiParticles)
    {
        if (isNewClump)
        {
            rigidBody.velocity = otherVelocity;
            rigidBody.angularVelocity = otherAngularVelocity;
            rigidBody.inertia = otherInertia;
        }
        else
        {
            rigidBody.inertia += (otherInertia + (newNumAntiParticles * newParticleRadius * newParticleRadius)); // Parallel axis theorem I_offset = I + md^2
            rigidBody.velocity = ((currentMass * currentVelocity) + (newNumAntiParticles * otherVelocity)) / (newNumAntiParticles + currentMass); // Conservation of linear momentum
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

    public void AddAntiParticle(GameObject antiParticle)
    {
        antiParticles.Add(antiParticle);
    }

    public bool FindMatchingAntiParticle(string particleName)
    {
        for (int i = antiParticles.Count; i-- > 0;)
        {
            if (antiParticles[i].tag == ANTI_PREFIX + particleName)
            {
                GameObject antiParticleToDestroy = antiParticles[i];
                antiParticles.RemoveAt(i);
                Destroy(antiParticleToDestroy);

                if (antiParticles.Count == 1)
                {
                    ReturnToFree();
                }
                else if (antiParticles.Count == 0)
                {
                    Destroy(gameObject);
                }

                return true;
            }
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D otherCollider)
    {
        if (otherCollider.gameObject.GetComponent<AntiParticleClump>())
        {
            if (!otherCollider.gameObject.GetComponent<AntiParticleClump>().touchedFirst)
            {
                touchedFirst = true;

                AddClumpToOtherClump(otherCollider.gameObject.GetComponent<AntiParticleClump>());
            }
        }
    }

    public void AddClumpToOtherClump(AntiParticleClump otherClump)
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
            otherClump.AddAntiParticle(child.gameObject);
        }
        otherClump.NewAngularMomentum(thisVelocity, thisAngularVelocity, thisInertia, false, thisDistance.magnitude, antiParticles.Count);

        Destroy(gameObject);
    }

    public void ReturnToFree()
    {
        Vector2 thisVelocity = rigidBody.velocity;
        float thisAngularVelocity = rigidBody.angularVelocity;

        GameObject remainingChild = antiParticles[0];

        remainingChild.transform.parent = transform.parent;
        Destroy(gameObject);

        remainingChild.AddComponent<ConstantForce2D>();

        Rigidbody2D newRigidBody = remainingChild.GetComponent<Rigidbody2D>();
        if (newRigidBody == null)
        {
            newRigidBody = remainingChild.AddComponent<Rigidbody2D>();
        }
        ParticleSpawner particleSpawner = FindObjectOfType<ParticleSpawner>();
        newRigidBody.angularDrag = particleSpawner.particleAngularDrag;
        newRigidBody.gravityScale = particleSpawner.particleGravityScale;
        newRigidBody.collisionDetectionMode = particleSpawner.particleCollisionDetectionMode;

        remainingChild.GetComponent<CircleCollider2D>().enabled = true;

        remainingChild.AddComponent<WaveRider>();
        remainingChild.AddComponent<FreeParticle>();

        AntiParticle newAntiParticle = remainingChild.AddComponent<AntiParticle>();

        newAntiParticle.rigidBody = newRigidBody;
        newAntiParticle.antiParticleClumpPrefab = FindObjectOfType<ParticleSpawner>().antiParticleClumpPrefab;

        newAntiParticle.rigidBody.velocity = thisVelocity;
        newAntiParticle.rigidBody.angularVelocity = thisAngularVelocity;
    }
}