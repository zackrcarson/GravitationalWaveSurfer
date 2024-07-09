using UnityEngine;

namespace GWS.Player.Runtime
{
    /// <summary>
    /// Rotates the player in the direction of movement. 
    /// </summary>
    public class RotateTowardsMovement: MonoBehaviour
    {
        [SerializeField] 
        private new Rigidbody rigidbody;

        [SerializeField, Min(0)] 
        private float rotationSpeed; 

        private void Update()
        {
            var velocityDirection = Vector3.Normalize(rigidbody.velocity);
            if (velocityDirection == Vector3.zero) return; 
            var targetRotation = Quaternion.LookRotation(velocityDirection, rigidbody.transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}