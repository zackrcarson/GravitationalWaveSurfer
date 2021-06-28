using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroBlackHole : MonoBehaviour
{
    // Config Parameters
    [SerializeField] GameObject pointerArrow = null;
    [SerializeField] BoxCollider2D screenEdgeCollider = null;

    [SerializeField] public float interactionRadius = 5f;
    [SerializeField] public float eventHorizon = 2f;
    [SerializeField] public float force = -70f;
    [SerializeField] public float eventHorizonForce = -1000f;

    [SerializeField] GameObject child = null;
    [SerializeField] float bufferSpace = 2f;

    [SerializeField] float[] timerMins = { 10f, 7f, 5f, 3f };
    [SerializeField] float[] timerMaxes = { 14f, 11f, 9f, 7f };

    [SerializeField] float scaleMin = .5f;
    [SerializeField] float scaleMax = 1.5f;

    [SerializeField] float speedMin = 5f;
    [SerializeField] float speedMax = 5f;

    // State Variables
    public bool isActive = false;
    bool hasStarted = false;
    bool hasShownArrow = false;
    public bool isGameOver = false;

    Vector2 velocity;
    float timer = 0f;

    // Cached References
    float xMin, xMax, yMin, yMax;
    float xMinScreen, xMaxScreen, yMinScreen, yMaxScreen;
    float xMinArrow, xMaxArrow, yMinArrow, yMaxArrow;
    Rigidbody2D rigidBody = null;
    float timerMin = 5f;
    float timerMax = 9f;

    // Constants
    const string PLAYER_TAG = "Player";
    const string BLACK_HOLE_NAME = "Black Hole";

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        hasShownArrow = false;
        hasStarted = false;
        isActive = false;

        child.SetActive(false);
        pointerArrow.SetActive(false);

        SetupMoveBoundaries();
    }

    // Start is called before the first frame update
    public void ExternalStart()
    {
        int difficulty = FindObjectOfType<GameManager>().difficulty;
        timerMin = timerMins[difficulty];
        timerMax = timerMaxes[difficulty];

        timer = Random.Range(timerMin, timerMax);

        hasStarted = true;
    }

    private void Update()
    {
        if (hasStarted)
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

        if (!isGameOver)
        {
            if (isActive && !CheckOnScreen())
            {
                RotateArrow();
            }
            else
            {
                pointerArrow.SetActive(false);
            }
        }
        else
        {
            pointerArrow.SetActive(false);
        }
    }

    private bool CheckOnScreen()
    {
        if (transform.position.x > xMinScreen && transform.position.x < xMaxScreen && transform.position.y > yMinScreen && transform.position.y < yMaxScreen)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CheckAndDeactivateBlackHole()
    {
        if (transform.position.x > xMax + (0.5 * bufferSpace) || transform.position.x < xMin - (0.5 * bufferSpace) || transform.position.y > yMax + (0.5 * bufferSpace) || transform.position.y < yMin - (0.5 * bufferSpace))
        {
            rigidBody.velocity = Vector2.zero;

            isActive = false;
            child.SetActive(false);
            hasShownArrow = false;

            timer = Random.Range(timerMin, timerMax);

            interactionRadius /= transform.localScale.x;
            eventHorizon /= transform.localScale.x;
            force /= transform.localScale.x;
        }
    }

    private void ActivateBlackHole()
    {
        velocity = GetPositionAndVelocity();

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

                vector1 = ((new Vector2(xMinArrow, yMaxArrow)) - (new Vector2(xPosition, yPosition))).normalized;
                vector2 = ((new Vector2(xMinArrow, yMinArrow)) - (new Vector2(xPosition, yPosition))).normalized;

                break;

            case 1:
                xPosition = Random.Range(xMin, xMax);
                yPosition = yMax;

                vector1 = ((new Vector2(xMinArrow, yMaxArrow)) - (new Vector2(xPosition, yPosition))).normalized;
                vector2 = ((new Vector2(xMaxArrow, yMaxArrow)) - (new Vector2(xPosition, yPosition))).normalized;

                break;

            case 2:
                xPosition = xMax;
                yPosition = Random.Range(yMin, yMax);

                vector1 = ((new Vector2(xMaxArrow, yMaxArrow)) - (new Vector2(xPosition, yPosition))).normalized;
                vector2 = ((new Vector2(xMaxArrow, yMinArrow)) - (new Vector2(xPosition, yPosition))).normalized;

                break;

            case 3:
                xPosition = Random.Range(xMin, xMax);
                yPosition = yMin;

                vector1 = ((new Vector2(xMinArrow, yMinArrow)) - (new Vector2(xPosition, yPosition))).normalized;
                vector2 = ((new Vector2(xMaxArrow, yMinArrow)) - (new Vector2(xPosition, yPosition))).normalized;

                break;

            default:
                xPosition = xMin;
                yPosition = yMax;

                vector1 = ((new Vector2(xMin + bufferSpace, yMax - bufferSpace)) - (new Vector2(xPosition, yPosition))).normalized;
                vector2 = ((new Vector2(xMin + bufferSpace, yMin + bufferSpace)) - (new Vector2(xPosition, yPosition))).normalized;

                Debug.LogError("Invalid side " + randomSide + " chosen.");
                break;
        }

        transform.position = new Vector2(xPosition, yPosition);

        return Random.Range(speedMin, speedMax) * (new Vector2(Random.Range(vector1.x, vector2.x), Random.Range(vector1.y, vector2.y)));
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (isActive)
        {
            if (otherCollider.gameObject.tag == PLAYER_TAG)
            {
                FindObjectOfType<Player>().KillPlayer(BLACK_HOLE_NAME);
            }
            else if (otherCollider.gameObject.GetComponent<Pickup>())
            {
                if (otherCollider.GetComponent<Rigidbody2D>() != null)
                {
                    otherCollider.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                }
                Destroy(otherCollider.gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, eventHorizon);
    }

    private void SetupMoveBoundaries()
    {
        Camera gameCamera = Camera.main;

        xMinScreen = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        xMaxScreen = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        yMinScreen = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        yMaxScreen = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;

        xMin = xMinScreen - bufferSpace;
        xMax = xMaxScreen + bufferSpace;
        yMin = yMinScreen - bufferSpace;
        yMax = yMaxScreen + bufferSpace;

        xMinArrow = screenEdgeCollider.offset.x - (screenEdgeCollider.size.x / 2f) - 0.01f;
        xMaxArrow = screenEdgeCollider.offset.x + (screenEdgeCollider.size.x / 2f) + 0.01f;
        yMinArrow = screenEdgeCollider.offset.y - (screenEdgeCollider.size.y / 2f) - 0.01f;
        yMaxArrow = screenEdgeCollider.offset.y + (screenEdgeCollider.size.y / 2f) + 0.01f;
    }

    private void RotateArrow()
    {
        Vector3 lookDirection = (transform.position - pointerArrow.transform.position).normalized;
        Vector3 newAngle = new Vector3(0f, 0f, 0f);

        if (lookDirection.x < 0 && lookDirection.y > 0)
        {
            newAngle.z = Mathf.Abs(Mathf.Rad2Deg * Mathf.Atan(lookDirection.x / lookDirection.y));
        }
        else if (lookDirection.x < 0 && lookDirection.y < 0)
        {
            newAngle.z = 90f + Mathf.Abs(Mathf.Rad2Deg * Mathf.Atan(lookDirection.y / lookDirection.x));
        }
        else if (lookDirection.x > 0 && lookDirection.y < 0)
        {
            newAngle.z = 180f + Mathf.Abs(Mathf.Rad2Deg * Mathf.Atan(lookDirection.x / lookDirection.y));
        }
        else if (lookDirection.x > 0 && lookDirection.y > 0)
        {
            newAngle.z = 270f + Mathf.Abs(Mathf.Rad2Deg * Mathf.Atan(lookDirection.y / lookDirection.x));
        }
        else if (lookDirection.x == 0 && lookDirection.y > 0)
        {
            newAngle.z = 0f;
        }
        else if (lookDirection.x < 0 && lookDirection.y == 0)
        {
            newAngle.z = 90f;
        }
        else if (lookDirection.x == 0 && lookDirection.y < 0)
        {
            newAngle.z = 180f;
        }
        else if (lookDirection.x > 0 && lookDirection.y == 0)
        {
            newAngle.z = 270f;
        }
        else
        {
            newAngle.z = 0f;
            Debug.LogError("Unknown angle with x = " + lookDirection.x + ", and y = " + lookDirection.y + ".");
        }

        pointerArrow.transform.localEulerAngles = newAngle;

        if (!pointerArrow.activeInHierarchy)
        {
            if (!hasShownArrow)
            {
                SetScreenPosition();

                pointerArrow.SetActive(true);
            
                hasShownArrow = true;
            }
        }
    }

    private void SetScreenPosition()
    {
        int layerMask = LayerMask.GetMask("Screen Edge");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, velocity, 20f, layerMask);
        
        if (hit.collider != null)
        {
            pointerArrow.transform.position = hit.point;
        }
        else
        {
            Debug.LogError("Direction Raycast did not hit screen edge");
            pointerArrow.transform.position = new Vector3(0f, -2.9f, 0f);
        }
    }
}
