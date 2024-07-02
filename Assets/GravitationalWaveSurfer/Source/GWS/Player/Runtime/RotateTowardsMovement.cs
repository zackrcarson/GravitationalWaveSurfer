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
            if (rigidbody.velocity.sqrMagnitude == 0) return; 
            var targetRotation = Quaternion.LookRotation(Vector3.Normalize(rigidbody.velocity), rigidbody.transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}