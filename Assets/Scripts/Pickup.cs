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

    // State variables
    bool hasTouched = false;
    [SerializeField] public List<string> listOfParticles;

    // Constants
    const string PROTON_NAME = "Proton";
    const string NEUTRON_NAME = "Neutron";
    const string ELECTRON_NAME = "Electron";
    const string PLAYER_NAME = "Player";

    private void Start()
    {
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
        if (!hasTouched)
        {
            if (otherCollider.gameObject.tag == PLAYER_NAME)
            {
                transform.parent = otherCollider.transform;

                Destroy(GetComponent<Rigidbody2D>());

                if (listOfParticles.Count > 0)
                {
                    GameManager.instance.AddParticles(listOfParticles);
                }

                hasTouched = true;
            }
            else if (otherCollider.gameObject.tag == PROTON_NAME || otherCollider.gameObject.tag == NEUTRON_NAME || otherCollider.gameObject.tag == ELECTRON_NAME)
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

        if (transform.parent.tag != "Untagged")
        {
            listOfParticles = new List<string>();
            Destroy(GetComponent<Rigidbody2D>());
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

            List<Pickup> listOfChildren = new List<Pickup>();
            GetAllChildren(gameObject, listOfChildren);
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
}
