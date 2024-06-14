using UnityEngine;

namespace GWS.GeneralRelativitySimulation.Runtime
{
    /// <summary>
    /// Maintains a collection of <see cref="ISimulatedObject"/> that should be simulated.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(SimulatedObjects), menuName = "ScriptableObjects/Simulation/Data Containers/Simulated Objects")]
    public class SimulatedObjects : ScriptableObject
    {
        /// <summary>
        /// The current list of objects that should be simulated.
        /// </summary>
        public ISimulatedObject[] objects;
    }
}