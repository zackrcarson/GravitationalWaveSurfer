using System.Collections.Generic;
using UnityEngine;

public class FreeParticle : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float initialRandomTorque = 0.001f;
    [SerializeField] float initialRandomPush = 0.4f;
    [SerializeField] float deadPlayerRandomTorque = 0.003f;
    [SerializeField] float deadPlayerRandomPush = 1.2f;

    // Cached References
    Rigidbody2D rigidBody = null;

    MicroBlackHole microBlackHole = null;
    new ConstantForce2D constantForce = null;
    Vector2 thisToBlackHole;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        microBlackHole = FindObjectOfType<MicroBlackHole>();

        if (TryGetComponent(out ConstantForce2D constantForce))
        {
            constantForce = GetComponent<ConstantForce2D>();
            constantForce.enabled = false;
            RandomKick(initialRandomTorque, initialRandomPush);

        }
        else
        {
            gameObject.AddComponent<ConstantForce2D>();
            constantForce = GetComponent<ConstantForce2D>();
            constantForce.enabled = false;

            RandomKick(deadPlayerRandomTorque, deadPlayerRandomPush);
        }
    }

    private void Update()
    {
        MicroBlackHole();
    }

    private void MicroBlackHole()
    {
        if (constantForce == null)
        {
            constantForce = GetComponent<ConstantForce2D>();
        }

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

    public void RandomKick(float torqueMagnitude, float forceMagnitude)
    {
        float randomRotation = torqueMagnitude * Random.Range(30f, 60f);
        rigidBody.AddTorque(randomRotation, ForceMode2D.Impulse);

        Vector2 randomPush = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rigidBody.velocity = forceMagnitude * randomPush;
    }
}