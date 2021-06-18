using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float initialRandomTorque = 0.001f;
    [SerializeField] float initialRandomPush = 0.4f;

    // Cached References
    Rigidbody2D rigidBody = null;
    float rigidBodyMass = 1f;
    float rigidBodyDrag = 0f;
    float rigidBodyAngularDrag = 0.001f;
    float rigidBodyGravityScale = 0f;
    CollisionDetectionMode2D rigidBodyCollisionDetectionMode;
    RigidbodyType2D rigidBodyBodyType;

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

        listOfParticles = new List<string>();
        listOfParticles.Add(tag);

        StoreRigidBody();

        RandomKick();
    }

    private void RandomKick()
    {
        float randomRotation = initialRandomTorque * Random.Range(30f, 60f);
        rigidBody.AddTorque(randomRotation, ForceMode2D.Impulse);

        Vector2 randomPush = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rigidBody.velocity = initialRandomPush * randomPush;
    }

    private void OnCollisionEnter2D(Collision2D otherCollider)
    {
        // If this particle hasn't been merged with the player yet. Else, do nothing.
        if (!hasTouched)
        {
            bool foundAnti = false;
            antiNames = new List<string>();

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
                    }
                }
            }

            // Check if the other collider is Player. If any anti exists, destroy both. Else, merge them.
            if (otherCollider.gameObject.tag == PLAYER_NAME)
            {
                if (foundAnti)
                {
                    //string victimName = antiNames[0].Replace(ANTI_PREFIX, "");
                    //FindObjectOfType<Player>().KillPlayer(victimName);

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

                    FindObjectOfType<Player>().AddParticle(this);

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
            if (foundAnti && ShouldDestroy(otherCollider))
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

    private IEnumerator FixChildStructure(Collision2D otherCollider)
    {
        yield return null;

        if (transform.parent.tag != PARTICLE_PARENT)
        {
            listOfParticles = new List<string>();
            Destroy(GetComponent<Rigidbody2D>());
            waveRider.canRide = false;
        }
        else
        {
            if (rigidBody == null)
            {
                AddRigidBody();
            }

            StopRigidBody();

            listOfParticles = new List<string>();
            listOfParticles.Add(tag);

            //List<Pickup> listOfChildren = new List<Pickup>();
            //GetAllChildren(gameObject, listOfChildren);
            foreach (Pickup child in listOfChildren)
            {
                listOfParticles.Add(child.gameObject.tag);
            }

            yield return null;

            RandomKick();
        }
    }

    private void StopRigidBody()
    {
        rigidBody.angularVelocity = 0f;
        rigidBody.velocity = Vector2.zero;
    }

    private void AddRigidBody()
    {
        rigidBody = gameObject.AddComponent<Rigidbody2D>();
        rigidBody.mass = rigidBodyMass;
        rigidBody.drag = rigidBodyDrag;
        rigidBody.angularDrag = rigidBodyAngularDrag;
        rigidBody.gravityScale = rigidBodyGravityScale;
        rigidBody.collisionDetectionMode = rigidBodyCollisionDetectionMode;
        rigidBody.bodyType = rigidBodyBodyType;
    }

    private void StoreRigidBody()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBodyMass = rigidBody.mass;
        rigidBodyDrag = rigidBody.drag;
        rigidBodyAngularDrag = rigidBody.angularDrag;
        rigidBodyGravityScale = rigidBody.gravityScale;
        rigidBodyCollisionDetectionMode = rigidBody.collisionDetectionMode;
        rigidBodyBodyType = rigidBody.bodyType;
    }

    private void GetAllChildren(GameObject obj, List<Pickup> childList)
    {
        if (obj == null || obj.GetComponent<Pickup>() == null)
        {
            return;
        }

        foreach (Transform child in obj.transform)
        {
            if (child == null || child.GetComponent<Pickup>() == null)
            {
                continue;
            }

            childList.Add(child.GetComponent<Pickup>());

            GetAllChildren(child.gameObject, childList);
        }
    }

    private void GetAllParents(GameObject obj, List<Pickup> parentList)
    {
        GameObject tobj = obj;

        if (obj != null && tobj.transform.parent != null)
        {
            while (tobj.transform.parent.tag != PARTICLE_PARENT)
            {
                tobj = tobj.transform.parent.gameObject;

                if (tobj == null)
                {
                    break;
                }

                parentList.Add(tobj.GetComponent<Pickup>());

                if (tobj.transform.parent == null)
                {
                    break;
                }
            }
        }
    }

    private void DestroyParent(GameObject obj)
    {
        if (obj.transform.parent.tag == PARTICLE_PARENT)
        {
            Destroy(obj);
        }
        else
        {
            Transform tempObject = obj.transform;

            while (tempObject.parent.tag != PARTICLE_PARENT)
            {
                tempObject = tempObject.transform.parent;
            }

            Destroy(tempObject.gameObject);
        }
    }

    /// <summary>
    /// Checks through the other clump of particles (parents, self, children) if there is a matching particle to the list of
    /// anti-particles in my clump of particles. The opposite will of course be checked from the OTHER clump of particles pickup script.
    /// </summary>
    /// <param name="otherCollider"></param>
    /// <returns></returns>
    private bool ShouldDestroy(Collision2D otherCollider)
    {
        // Check if the list of antis in the other particle matches the base particle non-anti
        if (antiNames.Contains(ANTI_PREFIX + tag))
        {
            return true;
        }

        // Check if the list of antis in the other particle matches the parent particles non-anti's
        List<Pickup> listOfMyParents = new List<Pickup>();
        GetAllParents(gameObject, listOfMyParents);
        foreach (Pickup parent in listOfMyParents)
        {
            if (parent == null) { continue; }

            if (antiNames.Contains(ANTI_PREFIX + parent.gameObject.tag))
            {
                return true;
            }
        }

        // Check if the list of antis in the other particle matches the children particles non-anti's
        List<Pickup> listOfMyChildren = new List<Pickup>();
        GetAllChildren(gameObject, listOfMyChildren);
        foreach (Pickup child in listOfMyChildren)
        {
            if (antiNames.Contains(ANTI_PREFIX + child.gameObject.tag))
            {
                return true;
            }
        }

        return false;
    }
}
