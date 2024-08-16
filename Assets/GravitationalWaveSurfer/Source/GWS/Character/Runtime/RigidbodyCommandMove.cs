using GWS.CommandPattern.Runtime;
using UnityEngine;

namespace GWS.Character.Runtime
{
    /// <summary>
    /// Command used to move a rigidbody  
    /// </summary>
    public readonly struct RigidbodyCommandMove: ICommand
    {
        private readonly Rigidbody rigidbody;
        private readonly Vector3 movement;
        private readonly Quaternion rotation; 
        private readonly float friction;

        /// <param name="rigidbody">The Rigidbody.</param>
        /// <param name="movement">The amount to add to velocity.</param>
        /// <param name="rotation">The angle by which to rotate the movement.</param>
        /// <param name="friction">A normalized value representing the percent of velocity that will be lost.</param>
        public RigidbodyCommandMove(Rigidbody rigidbody, Vector3 movement, Quaternion rotation, float friction)
            : this()
        {
            this.rigidbody = rigidbody;
            this.movement = movement;
            this.rotation = rotation;
            this.friction = friction;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Execute()   
        {
            // rigidbody.velocity = (rigidbody.velocity + rotation * movement) * (1 - friction);
            rigidbody.AddForce(rotation * movement, ForceMode.Impulse);
        }
    }
}