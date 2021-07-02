using System.Collections.Generic;
using UnityEngine;

public class AntiParticle : MonoBehaviour
{
    // Config Parameters
    [SerializeField] public GameObject antiParticleClumpPrefab = null;

    // Cached References
    public Rigidbody2D rigidBody = null;
    new ConstantForce2D constantForce = null;

    // State variables
    public bool touchedFirst = false;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        constantForce = GetComponent<ConstantForce2D>();
        constantForce.enabled = false;
    }

    public void AddToClump(AntiParticleClump clump = null, AntiParticle otherAntiParticle = null)
    {
        constantForce.enabled = false;
        Destroy(constantForce);

        Vector2 thisVelocity = rigidBody.velocity;
        float thisAngularVelocity = rigidBody.angularVelocity;
        float thisInertia = rigidBody.inertia;

        rigidBody.angularVelocity = 0f;
        rigidBody.velocity = Vector2.zero;
        Destroy(rigidBody);

        Destroy(GetComponent<WaveRider>());
        Destroy(GetComponent<FreeParticle>());

        if (clump == null)
        {
            clump = Instantiate(antiParticleClumpPrefab, transform.position, transform.rotation, transform.parent).GetComponent<AntiParticleClump>();
            clump.NewClump();
        }

        clump.AddAntiParticle(gameObject);

        clump.StoreCurrentAngularMomentum();
        transform.parent = clump.transform;
        clump.NewAngularMomentum(thisVelocity, thisAngularVelocity, thisInertia, otherAntiParticle != null, transform.localPosition.magnitude, 1);

        if (otherAntiParticle != null)
        {
            otherAntiParticle.AddToClump(clump);
        }

        Destroy(this);
    }

    private void OnCollisionEnter2D(Collision2D otherCollider)
    {
        if (otherCollider.gameObject.GetComponent<AntiParticle>())
        {
            if (!otherCollider.gameObject.GetComponent<AntiParticle>().touchedFirst)
            {
                touchedFirst = true;

                AddToClump(null, otherCollider.gameObject.GetComponent<AntiParticle>());
            }
        }
        else if (otherCollider.gameObject.GetComponent<AntiParticleClump>())
        {
            AddToClump(otherCollider.gameObject.GetComponent<AntiParticleClump>());
        }
    }
}
