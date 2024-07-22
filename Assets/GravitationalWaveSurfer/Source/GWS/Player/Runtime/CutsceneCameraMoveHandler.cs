using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCameraMoveHandler : MonoBehaviour
{
    [SerializeField]
    private float flySpeed = 10f;

    [SerializeField]
    private Camera playerCamera;

    private Vector3 moveDirection;
    private bool canMove = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (canMove)
        {
            HandleMouseLook();
            HandleMovementInput();
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * 2f;
        float mouseY = Input.GetAxis("Mouse Y") * 2f;

        transform.Rotate(Vector3.up * mouseX);
        playerCamera.transform.Rotate(Vector3.left * mouseY);
    }

    private void HandleMovementInput()
    {
        moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            moveDirection += playerCamera.transform.forward;
        if (Input.GetKey(KeyCode.S))
            moveDirection -= playerCamera.transform.forward;
        if (Input.GetKey(KeyCode.A))
            moveDirection -= playerCamera.transform.right;
        if (Input.GetKey(KeyCode.D))
            moveDirection += playerCamera.transform.right;

        moveDirection.Normalize();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Moving to next scene...");
        }
    }

    private void ApplyMovement()
    {
        transform.position += moveDirection * flySpeed * Time.fixedDeltaTime;
    }

    public void SetMove(bool state)
    {
        canMove = state;
    }
}
