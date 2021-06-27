using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroBlackHole : MonoBehaviour
{ BlackHole HOLE seems to be moving in the wrong direction.. Also properly kill player with new game over screen!!
    // config Parameters
    [SerializeField] public float interactionRadius = 5f;
    [SerializeField] public float eventHorizon = 2f;
    [SerializeField] public float force = -70f;
    [SerializeField] public float eventHorizonForce = -1000f;

    [SerializeField] GameObject child = null;
    [SerializeField] float bufferSpace = 2f;

    [SerializeField] float timerMin = 4f;
    [SerializeField] float timerMax = 8f;

    [SerializeField] float scaleMin = .5f;
    [SerializeField] float scaleMax = 1.5f;

    [SerializeField] float speedMin = 5f;
    [SerializeField] float speedMax = 5f;

    // State Variables
    public bool isActive = false;
    float timer = 0f;

    // Cached References
    float xMin, xMax, yMin, yMax;
    Rigidbody2D rigidBody = null;

    // Constants
    const string PLAYER_TAG = "Player";
    const string BLACK_HOLE_NAME = "Black Hole";

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        isActive = false;
        child.SetActive(false);

        SetupMoveBoundaries();
        timer = Random.Range(timerMin, timerMax);
    }

    private void Update()
    {
        if (!isActive)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                ActivateBlackHole();
            }
        }
        else
        {
            CheckAndDeactivateBlackHole();
        }
    }

    private void CheckAndDeactivateBlackHole()
    {
        if (transform.position.x > xMax + (0.5 * bufferSpace) || transform.position.x < xMin - (0.5 * bufferSpace) || transform.position.y > yMax + (0.5 * bufferSpace) || transform.position.y < yMin - (0.5 * bufferSpace))
        {
            rigidBody.velocity = Vector2.zero;

            isActive = false;
            child.SetActive(false);
            timer = Random.Range(timerMin, timerMax);
        }
    }

    private void ActivateBlackHole()
    {
        Vector2 velocity = GetPositionAndVelocity();

        float scale = Random.Range(scaleMin, scaleMax);
        transform.localScale = new Vector2(scale, scale);

        interactionRadius *= transform.localScale.x;
        eventHorizon *= transform.localScale.x;
        force *= transform.localScale.x;

        isActive = true;
        child.SetActive(true);

        rigidBody.velocity = velocity;
    }

    private Vector2 GetPositionAndVelocity()
    {
        int randomSide = Random.Range(0, 3);
        float xPosition, yPosition;

        Vector2 vector1, vector2;

        switch (randomSide)
        {
            case 0:
                xPosition = xMin;
                yPosition = Random.Range(yMin, yMax);

                vector1 = (new Vector2(xMin + bufferSpace, yMax - bufferSpace)) - (new Vector2(xPosition, yPosition)).normalized;
                vector2 = (new Vector2(xMin + bufferSpace, yMin + bufferSpace)) - (new Vector2(xPosition, yPosition)).normalized;

                break;

            case 1:
                xPosition = Random.Range(xMin, xMax);
                yPosition = yMax;

                vector1 = (new Vector2(xMin + bufferSpace, yMax - bufferSpace)) - (new Vector2(xPosition, yPosition)).normalized;
                vector2 = (new Vector2(xMax - bufferSpace, yMax - bufferSpace)) - (new Vector2(xPosition, yPosition)).normalized;

                break;

            case 2:
                xPosition = xMax;
                yPosition = Random.Range(yMin, yMax);

                vector1 = (new Vector2(xMax - bufferSpace, yMax - bufferSpace)) - (new Vector2(xPosition, yPosition)).normalized;
                vector2 = (new Vector2(xMax - bufferSpace, yMin + bufferSpace)) - (new Vector2(xPosition, yPosition)).normalized;

                break;

            case 3:
                xPosition = Random.Range(xMin, xMax);
                yPosition = yMin;

                vector1 = (new Vector2(xMin + bufferSpace, yMin + bufferSpace)) - (new Vector2(xPosition, yPosition)).normalized;
                vector2 = (new Vector2(xMax - bufferSpace, yMin + bufferSpace)) - (new Vector2(xPosition, yPosition)).normalized;

                break;

            default:
                xPosition = xMin;
                yPosition = yMax;

                vector1 = (new Vector2(xMin + bufferSpace, yMax - bufferSpace)) - (new Vector2(xPosition, yPosition)).normalized;
                vector2 = (new Vector2(xMin + bufferSpace, yMin + bufferSpace)) - (new Vector2(xPosition, yPosition)).normalized;

                Debug.LogError("Invalid side " + randomSide + " chosen.");
                break;
        }

        transform.position = new Vector2(xPosition, yPosition);

        return Random.Range(speedMin, speedMax) * (new Vector2(Random.Range(vector1.x, vector2.x), Random.Range(vector1.y, vector2.y)));
    }

    private void OnCollisionEnter2D(Collision2D otherCollider)
    {
        if (isActive)
        {
            if (otherCollider.gameObject.tag == PLAYER_TAG)
            {
                Destroy(otherCollider.gameObject);
                // otherCollider.gameObject.GetComponent<Player>().KillPlayer(BLACK_HOLE_NAME);
            }
            else if (otherCollider.gameObject.GetComponent<Pickup>())
            {
                Destroy(otherCollider.gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRadius * transform.localScale.x);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, eventHorizon * transform.localScale.x);
    }

    private void SetupMoveBoundaries()
    {
        Camera gameCamera = Camera.main;

        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x - bufferSpace;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + bufferSpace;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y - bufferSpace;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y + bufferSpace;
    }
}
