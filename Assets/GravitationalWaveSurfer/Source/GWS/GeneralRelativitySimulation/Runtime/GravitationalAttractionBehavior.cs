using System.Collections.Generic;
using GWS.GeneralRelativity.Runtime;
using UnityEngine;

namespace GWS.GeneralRelativitySimulation.Runtime
{
    /// <summary>
    /// Simulates gravitational attraction between a simulated object and all other simulated objects.
    /// </summary>
    public class GravitationalAttractionBehavior: MonoBehaviour
    {
        /// <summary>
        /// The simulated objects
        /// </summary>
        public SimulatedObjects simulatedObjects;
        
        /// <summary>
        /// The object to simulate.
        /// </summary>
        [SerializeField]
        public SimulatedObjectMonoBehaviour simulatedObject;

        [SerializeField] 
        private float forceMultiplier = 1;

        private void FixedUpdate()
        {
            var totalForces = GetTotalForces(simulatedObject, simulatedObjects.objects); 
            simulatedObject.AddForce(totalForces * forceMultiplier);
        }

        private static Vector3 GetTotalForces(ISimulatedObject simulatedObject, IEnumerable<ISimulatedObject> otherObjects)
        {
            var totalForces = Vector3.zero;
            
            foreach (var other in otherObjects)
            {
                if (other == null || simulatedObject.InstanceID == other.InstanceID) continue;
                var distance = Vector3.Distance(simulatedObject.Position, other.Position) * GRPhysics.MetersToGigameters;
                var direction = GRPhysics.EvaluateGravitationalForceDirection(simulatedObject.Position, other.Position, 
                    other.RotationDelta);
                var force = GRPhysics.EvaluateGravitationalForceMagnitude(simulatedObject.Mass, other.Mass, distance) 
                    * GRPhysics.KilogramsToRonnagrams / simulatedObject.Mass;
                totalForces += direction * (float)force;
            }

            return totalForces;
        }
    }
}