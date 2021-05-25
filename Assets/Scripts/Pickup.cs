using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float initialRandomTorque = 0.001f;
    [SerializeField] float initialRandomPush = 0.4f;

    // Cached References
    Rigidbody2D rigidBody = null;

    // State variables
    bool hasTouched = false;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

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
            if (otherCollider.gameObject.tag == "Player")
            {
                transform.parent = otherCollider.transform;

                Destroy(GetComponent<Rigidbody2D>());

                GameManager.instance.AddParticle(tag);

                hasTouched = true;
            }
        }
    }
}
