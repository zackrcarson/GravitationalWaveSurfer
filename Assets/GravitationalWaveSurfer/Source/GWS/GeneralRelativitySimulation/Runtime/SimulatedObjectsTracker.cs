using UnityEngine;

namespace GWS.GeneralRelativitySimulation.Runtime
{
    /// <summary>
    /// Updates a <see cref="SimulatedObjects"/> container, based on <see cref="ISimulatedObject"/> within a certain radius.
    /// </summary>
    public class SimulatedObjectsTracker: MonoBehaviour
    {
        /// <summary>
        /// The <see cref="SimulatedObjects"/> to update. 
        /// </summary>
        [SerializeField]
        private SimulatedObjects simulatedObjects; 
        
        /// <summary>
        /// The radius in which this is attracted to other objects.
        /// </summary>
        [SerializeField, Min(0)]
        private float interactionRadius;
        
        /// <summary>
        /// The active collisions within the <see cref="interactionRadius"/>.
        /// </summary>
        [SerializeField]
        private Collider[] collisions;
        
        /// <summary>
        /// The maximum amount of collisions allowed at once.
        /// </summary>
        private const int MaxCollisionCount = 20;

        private void Awake()
        {
            collisions = new Collider[MaxCollisionCount];
            simulatedObjects.objects = new ISimulatedObject[MaxCollisionCount];
        }

        private void FixedUpdate()
        {
            Physics.OverlapSphereNonAlloc(transform.position, interactionRadius, collisions);
            UpdateSimulatedObjects();
        }

        private void UpdateSimulatedObjects()
        {
            for (var i = 0; i < collisions.Length; i++)
            {
                if (!collisions[i] || collisions[i].GetComponent<ISimulatedObject>() is not { } other)
                {
                    simulatedObjects.objects[i] = null;
                    continue;
                }
                simulatedObjects.objects[i] = other;
            }
        }
    }
}