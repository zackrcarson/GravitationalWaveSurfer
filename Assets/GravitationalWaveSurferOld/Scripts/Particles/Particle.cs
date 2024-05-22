using UnityEngine;

public class Particle : MonoBehaviour
{
    // Config Parameters
    [SerializeField] public GameObject particleClumpPrefab = null;

    // Cached References
    public Rigidbody2D rigidBody = null;
    new ConstantForce2D constantForce = null;
    private Highlight particleHighlight = null;

    // State variables
    public bool touchedFirst = false;

    // Constants
    const string ANTI_PREFIX = "Anti-";

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        constantForce = GetComponent<ConstantForce2D>();
        particleHighlight = GetComponent<Highlight>();

        constantForce.enabled = false;
    }

    public void AddToClump(ParticleClump clump = null, Particle otherParticle = null)
    {
        if (constantForce != null)
        {
            constantForce.enabled = false;
        }
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
            clump = Instantiate(particleClumpPrefab, transform.position, transform.rotation, transform.parent).GetComponent<ParticleClump>();
            clump.NewClump();
        }

        clump.AddParticle(gameObject);

        clump.StoreCurrentAngularMomentum();
        transform.parent = clump.transform;
        clump.NewAngularMomentum(thisVelocity, thisAngularVelocity, thisInertia, otherParticle != null, transform.localPosition.magnitude, 1);

        if (otherParticle != null)
        {
            otherParticle.AddToClump(clump);
        }

        Destroy(this);
    }

    public void AddToPlayer(Player player)
    {
        if (constantForce != null)
        {
            constantForce.enabled = false;
        }
        Destroy(constantForce);

        if (rigidBody != null)
        {
            rigidBody.angularVelocity = 0f;
            rigidBody.velocity = Vector2.zero;
            Destroy(rigidBody);
        }

        Destroy(GetComponent<WaveRider>());
        Destroy(GetComponent<FreeParticle>());

        player.AddParticle(gameObject);

        transform.parent = player.transform;

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
            if (otherCollider.gameObject.tag == ANTI_PREFIX + tag)
            {
                Destroy(otherCollider.gameObject);
                Destroy(gameObject);

                // TODO: Particle Effect
            }
        }
        else if (otherCollider.gameObject.GetComponent<AntiParticleClump>())
        {
            if (otherCollider.gameObject.GetComponent<AntiParticleClump>().FindMatchingAntiParticle(tag))
            {
                Destroy(gameObject);

                // TODO: Particle Effect
            }
        }
        else if (otherCollider.gameObject.GetComponent<Player>())
        {
            Player player = otherCollider.gameObject.GetComponent<Player>();
            
            particleHighlight.ToggleHighlight(true);

            AddToPlayer(player);
        }
    }
}