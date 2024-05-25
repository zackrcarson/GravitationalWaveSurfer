// File: Assets/Scripts/Management/CameraManagement.cs

using UnityEngine;

public class CameraManagement : MonoBehaviour
{
    // Variables for rotation
    public Transform mainObject; // The main object to which the camera is attached
    public float rotationSpeed = 50f; // Speed of the camera rotation
    private Vector3 lastMousePosition;
    public bool inverted = false;

    // Variables for zoom
    public float zoomSpeed = 100.0f;
    public float minFOV = 20.0f;
    public float maxFOV = 100.0f;

    private Camera cam;

    void Start()
    {
        if (mainObject == null)
        {
            Debug.LogError("Main object (parent of camera) not assigned.");
            return;
        }

        cam = this.GetComponent<Camera>();
        if (GetComponent<Camera>() == null)
        {
            Debug.LogError("Camera component not found.");
            return;
        }

        if (!GetComponent<Camera>().orthographic)
        {
            GetComponent<Camera>().fieldOfView = Mathf.Clamp(GetComponent<Camera>().fieldOfView, minFOV, maxFOV);
        }
        else
        {
            Debug.LogError("This script is intended for perspective cameras only.");
        }
    }

    void Update()
    {
        HandleMouseRotation();
        HandleMouseZoom();
    }

    void HandleMouseRotation()
    {
        // Check if the right mouse button is pressed
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("mouse rotation");
            lastMousePosition = Input.mousePosition;
        }

        // Check if the right mouse button is being held down
        if (Input.GetMouseButton(1))
        {
            // Calculate the mouse movement delta
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            // Calculate rotation around the Y axis (horizontal rotation)
            float rotation_x = -mouseDelta.x * rotationSpeed * Time.deltaTime;
            // Calculate rotation around the X axis (vertical rotation)
            float rotation_y = mouseDelta.y * rotationSpeed * Time.deltaTime;

            if (inverted)
            {
                rotation_x *= -1f;
                rotation_y *= -1f;
            }

            // Apply the rotations to the main object
            mainObject.transform.Rotate(Vector3.up, rotation_x, Space.World);
            mainObject.transform.Rotate(Vector3.right, rotation_y, Space.Self);
        }
    }

    void HandleMouseZoom()
    {
        // Get the mouse wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f)
        {
            Debug.Log(scrollInput);
            // Adjust the orthographic size based on the mouse wheel input
            cam.fieldOfView -= scrollInput * zoomSpeed * cam.fieldOfView * Time.deltaTime;

            // Clamp the orthographic size to stay within the min and max zoom limits
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }
    }
}
