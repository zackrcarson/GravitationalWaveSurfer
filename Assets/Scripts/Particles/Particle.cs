using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    // Config Parameters
    [SerializeField] GameObject particleClumpPrefab = null;

    // Cached References
    Rigidbody2D rigidBody = null;
    new ConstantForce2D constantForce = null;

    // State variables
    public bool touchedFirst = false;
    [SerializeField] public List<string> listOfParticles;
    [SerializeField] public List<Pickup> listOfChildren = new List<Pickup>();
    [SerializeField] public List<Pickup> listOfParents = new List<Pickup>();
    [SerializeField] public List<string> antiNames = new List<string>();

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        constantForce = GetComponent<ConstantForce2D>();

        listOfParticles = new List<string>();
        listOfParticles.Add(tag);
    }

    public void AddToClump(ParticleClump clump = null, Particle otherParticle = null)
    {
        constantForce.enabled = false;
        Destroy(constantForce);

        rigidBody.angularVelocity = 0f;
        rigidBody.velocity = Vector2.zero;
        Destroy(rigidBody);

        Destroy(GetComponent<WaveRider>());
        Destroy(GetComponent<FreeParticle>());

        if (clump == null)
        {
            clump = Instantiate(particleClumpPrefab, transform.position, transform.rotation, transform.parent).GetComponent<ParticleClump>();
            clump.NewClump();
        }

        clump.AddParticle(tag, otherParticle == null);
        transform.parent = clump.transform;

        if (otherParticle != null)
        {
            otherParticle.AddToClump(clump);
        }

        Destroy(this);
    }

    private void OnCollisionEnter2D(Collision2D otherCollider)
    {
        if (otherCollider.gameObject.GetComponent<Particle>())
        {
            if (!otherCollider.gameObject.GetComponent<Particle>().touchedFirst)
            {
                touchedFirst = true;

                AddToClump(null, otherCollider.gameObject.GetComponent<Particle>());
            }
        }
        else if (otherCollider.gameObject.GetComponent<ParticleClump>())
        {
            AddToClump(otherCollider.gameObject.GetComponent<ParticleClump>());
        }
        else if (otherCollider.gameObject.GetComponent<AntiParticle>())
        {

        }
        else if (otherCollider.gameObject.GetComponent<AntiParticleClump>() || otherCollider.gameObject.GetComponentInParent<AntiParticleClump>())
        {

        }
        else if (otherCollider.gameObject.GetComponent<Player>() || otherCollider.gameObject.GetComponentInParent<Player>())
        {

        }
    }
}
