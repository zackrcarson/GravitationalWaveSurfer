using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float playerSpeed = 1f;
    [SerializeField] float initialRandomTorque = 0.001f;
    [SerializeField] float initialRandomPush = 0.4f;

    // Cached References
    Rigidbody2D rigidBody = null;

    bool canMove = true;
    bool isMoving = false;
    bool hasStartedControlling = false;

    // Start is called before the first frame update
    void Start()
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
    }

    ///*private void OnTriggerEnter2D(Collider2D otherCollider)*/
    //private void OnCollisionEnter2D(Collision2D otherCollider)
    //{
    //    Debug.Log(otherCollider);

    //    // creates joint
    //    FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();

    //    // sets joint position to point of contact
    //    joint.anchor = otherCollider.contacts[0].point;

    //    // conects the joint to the other object
    //    joint.connectedBody = otherCollider.contacts[0].otherCollider.transform.GetComponentInParent<Rigidbody2D>();

    //    // Stops objects from continuing to collide and creating more joints
    //    joint.enableCollision = false;
    //}
}
