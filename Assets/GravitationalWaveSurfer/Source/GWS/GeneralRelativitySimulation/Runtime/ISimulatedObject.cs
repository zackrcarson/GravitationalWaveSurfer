using UnityEngine;

namespace GWS.GeneralRelativitySimulation.Runtime
{
    /// <summary>
    /// An object that will be simulated.
    /// </summary>
    public interface ISimulatedObject
    {
        /// <summary>
        /// The instance id of this object.
        /// </summary>
        public int InstanceID { get; }
        
        /// <summary>
        /// The position of the object. 
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Adds a force.
        /// </summary>
        /// <param name="force"></param>
        public void AddForce(Vector3 force);
        
        /// <summary>
        /// The mass of the object in Ronnagrams, which is 10e+27 Kilograms. 
        /// </summary>
        /// <remarks>Default measurement of objects in unity is Kilograms.</remarks>
        public double Mass { get; set; }
        
        /// <summary>
        /// The change in rotation per fixed update step.
        /// </summary>
        public Quaternion RotationDelta { get; set;  }
    }
}