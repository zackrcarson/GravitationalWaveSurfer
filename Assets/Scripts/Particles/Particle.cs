using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float initialRandomTorque = 0.001f;
    [SerializeField] float initialRandomPush = 0.4f;

    [SerializeField] GameObject particleClumpPrefab = null;

    // Cached References
    Rigidbody2D rigidBody = null;

    MicroBlackHole microBlackHole = null;
    new ConstantForce2D constantForce = null;
    Vector2 thisToBlackHole;

    WaveRider waveRider = null;

    // State variables
    bool hasTouched = false;
    [SerializeField] public List<string> listOfParticles;
    [SerializeField] public List<Pickup> listOfChildren = new List<Pickup>();
    [SerializeField] public List<Pickup> listOfParents = new List<Pickup>();
    [SerializeField] public List<string> antiNames = new List<string>();

    // Constants
    const string PROTON_NAME = "Proton";
    const string NEUTRON_NAME = "Neutron";
    const string ELECTRON_NAME = "Electron";
    const string ANTI_PREFIX = "Anti-";
    const string PLAYER_NAME = "Player";
    const string PARTICLE_PARENT = "Particle_Parent";

    private void Start()
    {
        waveRider = GetComponent<WaveRider>();
        rigidBody = GetComponent<Rigidbody2D>();

        listOfParticles = new List<string>();
        listOfParticles.Add(tag);
    }

    private void AddToClump(ParticleClump clump = null)
    {
        if (rigidBody != null)
        {
            rigidBody.angularVelocity = 0f;
            rigidBody.velocity = Vector2.zero;
        }
        if (constantForce != null)
        {
            constantForce.enabled = false;
            Destroy(constantForce);
        }

        Destroy(rigidBody);
        Destroy(waveRider);

        if (clump == null)
        {
            clump = Instantiate(particleClumpPrefab, transform).GetComponent<ParticleClump>();
        }

        clump.AddParticle(tag);
        transform.parent = clump.transform;

        Destroy(this);
    }










    private void OnCollisionEnter2D(Collision2D otherCollider)
    {
        if (otherCollider.gameObject.GetComponent<Particle>())
        {

        }
        else if (otherCollider.gameObject.GetComponent<AntiParticle>())
        {

        }
        else if (otherCollider.gameObject.GetComponent<ParticleClump>() || otherCollider.gameObject.GetComponentInParent<ParticleClump>())
        {

        }
        else if (otherCollider.gameObject.GetComponent<AntiParticleClump>() || otherCollider.gameObject.GetComponentInParent<AntiParticleClump>())
        {

        }
        else if (otherCollider.gameObject.GetComponent<Player>() || otherCollider.gameObject.GetComponentInParent<Player>())
        {

        }
        else
        {
            Debug.LogError(tag + " collided with unknown object " + otherCollider.gameObject.tag + ".");
        }


        // If this particle hasn't been merged with the player yet. Else, do nothing.
        if (!hasTouched)
        {
            bool foundAnti = false;
            antiNames = new List<string>();
            List<Pickup> antiParticles = new List<Pickup>();

            listOfChildren = new List<Pickup>();
            listOfParents = new List<Pickup>();

            // Collect list of children in other object
            GetAllChildren(gameObject, listOfChildren);

            // Collect list of parents in other object
            GetAllParents(gameObject, listOfParents);

            // Check if the other particle is anti
            if (tag.StartsWith(ANTI_PREFIX))
            {
                foundAnti = true;
                antiNames.Add(tag);
                antiParticles.Add(this);
            }

            // Check if any of the children of the other particle is an anti
            if (listOfChildren.Count > 0)
            {
                foreach (Pickup child in listOfChildren)
                {
                    if (child.gameObject.tag.StartsWith(ANTI_PREFIX))
                    {
                        foundAnti = true;
                        antiNames.Add(child.gameObject.tag);
                        antiParticles.Add(child);
                    }
                }
            }

            // Check if any of the parents of the other particle is an anti
            if (listOfParents.Count > 0)
            {
                foreach (Pickup parent in listOfParents)
                {
                    if (parent == null) { continue; }

                    if (parent.gameObject.tag.StartsWith(ANTI_PREFIX))
                    {
                        foundAnti = true;
                        antiNames.Add(parent.gameObject.tag);
                        antiParticles.Add(parent);
                    }
                }
            }

            // Check if the other collider is Player. If any anti exists, destroy both particles only. Else, merge them.
            if (otherCollider.gameObject.tag == PLAYER_NAME)
            {
                if (foundAnti)
                {
                    // Replace below with above to change to automatically dying when hitting an anti particle
                    bool shouldDestroy = FindObjectOfType<Player>().AnnihilateParticles(antiNames);

                    if (shouldDestroy)
                    {
                        DestroyParent(gameObject);
                    }
                }
                else
                {
                    transform.parent = otherCollider.transform;

                    tag = PLAYER_NAME;

                    FindObjectOfType<Player>().AddParticle(this);

                    if (constantForce != null)
                    {
                        constantForce.enabled = false;
                        Destroy(constantForce);
                    }
                    Destroy(GetComponent<Rigidbody2D>());

                    if (listOfParticles.Count > 0)
                    {
                        GameManager.instance.AddParticles(listOfParticles);
                    }

                    hasTouched = true;
                    waveRider.canRide = false;
                }

                return;
            }

            // Check if we should destroy both particles if there is a matching anti/non-anti in either clump!
            if (foundAnti && ShouldDestroy())
            {
                DestroyParent(gameObject);
                DestroyParent(otherCollider.gameObject);

                // TODO: Destroy particle effect 

                return;
            }
            else
            {
                if (rigidBody != null)
                {
                    StopRigidBody();
                }

                transform.parent = otherCollider.transform;

                StartCoroutine(FixChildStructure(otherCollider));
            }
        }
    }
}
