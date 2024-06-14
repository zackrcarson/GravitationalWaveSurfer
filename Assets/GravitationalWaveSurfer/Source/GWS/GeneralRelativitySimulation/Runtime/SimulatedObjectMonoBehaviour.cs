using UnityEngine;

namespace GWS.GeneralRelativitySimulation.Runtime
{
    /// <summary>
    /// Abstract <see cref="MonoBehaviour"/> that implements <see cref="ISimulatedObject"/>.
    /// Extend from this class directly instead of <see cref="ISimulatedObject"/> so that it can be assigned to in the editor. 
    /// </summary>
    public abstract class SimulatedObjectMonoBehaviour : MonoBehaviour, ISimulatedObject
    {
        public abstract int InstanceID { get; }
        public abstract Vector3 Position { get; set; }
        public abstract double Mass { get; set; }
        public abstract Quaternion RotationDelta { get; set; }
        public abstract void AddForce(Vector3 forcd);
    }
}