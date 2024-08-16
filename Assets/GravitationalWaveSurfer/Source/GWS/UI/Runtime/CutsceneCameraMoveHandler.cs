using UnityEngine;
using UnityEngine.SceneManagement;

namespace GWS.UI.Runtime
{
    /// <summary>
    /// Used in cutscenes only. Allows the player to control the camera.
    /// </summary>
    public class CutsceneCameraMoveHandler : MonoBehaviour
    {
        [SerializeField]
        private float flySpeed = 10f;

        [SerializeField]
        private Camera playerCamera;

        private float mouseSensitivty = 2f;

        private Vector3 moveDirection;
        private bool canMove = false;

        private void Start()
        {
            //Cursor.lockState = CursorLockMode.Locked;
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

        // TODO - move to new output system
        private void HandleMouseLook()
        {
            float mouseX = UnityEngine.Input.GetAxis("Mouse X") * mouseSensitivty;
            float mouseY = UnityEngine.Input.GetAxis("Mouse Y") * mouseSensitivty;

            transform.Rotate(Vector3.up * mouseX);
            playerCamera.transform.Rotate(Vector3.left * mouseY);
        }

        // TODO - move to new output system
        private void HandleMovementInput()
        {
            moveDirection = Vector3.zero;

            if (UnityEngine.Input.GetKey(KeyCode.W))
                moveDirection += playerCamera.transform.forward;
            if (UnityEngine.Input.GetKey(KeyCode.S))
                moveDirection -= playerCamera.transform.forward;
            if (UnityEngine.Input.GetKey(KeyCode.A))
                moveDirection -= playerCamera.transform.right;
            if (UnityEngine.Input.GetKey(KeyCode.D))
                moveDirection += playerCamera.transform.right;

            moveDirection.Normalize();

            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("BaseMainMenu");
            }
        }

        private void ApplyMovement()
        {
            transform.position += moveDirection * (flySpeed * Time.fixedDeltaTime);
        }

        /// <summary>
        /// Used by the AllowCameraMovement signal asset.
        /// </summary>
        /// <param name="state"></param>
        public void SetMove(bool state)
        {
            canMove = state;
        }
    }

}
