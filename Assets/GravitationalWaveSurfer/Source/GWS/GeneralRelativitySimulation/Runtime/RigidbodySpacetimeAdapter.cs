using System;
using UnityEngine;

namespace GWS.GeneralRelativitySimulation.Runtime
{
    /// <summary>
    /// Adapts a rigidbody to implement various interfaces needed for the simulation. Can be used the same way as a normal rigidbody.
    /// </summary>
    public class RigidbodySpacetimeAdapter: SimulatedObjectMonoBehaviour
    {
        public override int InstanceID => gameObject.GetInstanceID();
        
        /// <summary>
        /// The <see cref="Rigidbody"/>.
        /// </summary>
        public new Rigidbody rigidbody;

        /// <summary>
        /// <inheritdoc cref="Rigidbody.position"/>
        /// </summary>
        /// <remarks>Equivalent to Rigidbody.position.</remarks>
        public override Vector3 Position
        {
            get => rigidbody.position;
            set => rigidbody.MovePosition(value);
        }
            
        /// <summary>
        /// <inheritdoc cref="Rigidbody.mass"/>
        /// </summary>
        /// <remarks>Equivalent to Rigidbody.mass.</remarks>
        public override double Mass 
        { 
            get => rigidbody.mass;
            set => rigidbody.mass = (float)value;
        }

        public override Quaternion RotationDelta { get; set; }

        private Quaternion lastRotation;

        private void Start()
        {
            lastRotation = transform.rotation;
        }

        private void FixedUpdate()
        {
            // https://forum.unity.com/threads/get-the-difference-between-two-quaternions-and-add-it-to-another-quaternion.513187/
            var rotation = transform.rotation;
            RotationDelta = lastRotation * Quaternion.Inverse(rotation);
            lastRotation = rotation;
        }

        public override void AddForce(Vector3 force)
        {
            rigidbody.AddForce(force, ForceMode.VelocityChange);
        }
    }
}