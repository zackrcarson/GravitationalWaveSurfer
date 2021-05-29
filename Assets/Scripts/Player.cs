using UnityEngine;

public class Player : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float playerSpeed = 1f;
    [SerializeField] float initialRandomTorque = 0.001f;
    [SerializeField] float initialRandomPush = 0.4f;
    [SerializeField] float xBoundaryPadding = 0.1f;
    [SerializeField] float yBoundaryPadding = 0.1f;

    // Cached References
    Rigidbody2D rigidBody = null;
    float xMin, xMax, yMin, yMax;

    bool canMove = true;
    bool isMoving = false;
    bool hasStartedControlling = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        
        SetupMoveBoundaries();
        
        RandomKick();
    }

    private void RandomKick()
    {
        float randomRotation = initialRandomTorque * Random.Range(30f, 60f);
        rigidBody.AddTorque(randomRotation, ForceMode2D.Impulse);

        Vector2 randomPush = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rigidBody.velocity = initialRandomPush * randomPush;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            Move();
        }
    }

    private void Move()
    {
        float currentInputX = Input.GetAxis("Horizontal");
        float currentInputY = Input.GetAxis("Vertical");

        if (currentInputX != 0 || currentInputY != 0)
        {
            hasStartedControlling = true;
        }

        if (hasStartedControlling)
        {
            float currentVelocity = playerSpeed;

            Vector2 newVelocity = new Vector2(currentVelocity * currentInputX, currentVelocity * currentInputY);

            rigidBody.velocity = newVelocity;

            isMoving = (Mathf.Abs(rigidBody.velocity.x) > Mathf.Epsilon) || (Mathf.Abs(rigidBody.velocity.y) > Mathf.Epsilon);
        }

        ClampPosition();
    }

    private void ClampPosition()
    {
        if (transform.position.x > xMax)
        {
            Vector3 newPos = new Vector3(xMax, transform.position.y, transform.position.z);
            transform.position = newPos;
        }

        if (transform.position.x < xMin)
        {
            Vector3 newPos = new Vector3(xMin, transform.position.y, transform.position.z);
            transform.position = newPos;
        }

        if (transform.position.y > yMax)
        {
            Vector3 newPos = new Vector3(transform.position.x, yMax, transform.position.z);
            transform.position = newPos;
        }

        if (transform.position.y < yMin)
        {
            Vector3 newPos = new Vector3(transform.position.x, yMin, transform.position.z);
            transform.position = newPos;
        }
    }

    private void SetupMoveBoundaries()
    {
        Camera gameCamera = Camera.main;

        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + xBoundaryPadding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - xBoundaryPadding;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + yBoundaryPadding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - yBoundaryPadding;
    }
}
