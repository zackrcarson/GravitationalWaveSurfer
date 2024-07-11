using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GWS.HydrogenCollection.Runtime
{
    /// <summary>
    /// Attracts hydrogen bobs and moves them towards the bob of the player.
    /// </summary>
    public class BobCollection : MonoBehaviour
    {
        [SerializeField] 
        private ParticleInventory particleInventory;

        /// <summary>
        /// The base radius of the bob.
        /// </summary>
        [SerializeField] 
        private float baseRadius; 
        
        [SerializeField]
        private GameObject radiusVisual;

        public static HashSet<Transform> attractedObjects = new HashSet<Transform>();

        private new SphereCollider collider;

        [SerializeField]
        private float colliderRadiusMultiplier = 20f;

        private void Start()
        {
            collider = GetComponent<SphereCollider>();
        }

        private void FixedUpdate()
        {
            foreach (var obj in attractedObjects.Where(obj => obj != null))
            {
                obj.position = Vector3.MoveTowards(obj.position, transform.position, 0.09f);
                if (obj.TryGetComponent<JitterEffect>(out var jitterEffect))
                {
                    jitterEffect.targetPosition = transform.position;
                }
            }
    
            var radius = colliderRadiusMultiplier * particleInventory.HydrogenCount + baseRadius;
            collider.radius = radius;
            radiusVisual.transform.localScale = Vector3.one * radius;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsAttractable(other))
            {
                attractedObjects.Add(other.transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsAttractable(other))
            {
                if (other.TryGetComponent<JitterEffect>(out var jitterEffect))
                {
                    jitterEffect.originalPosition = jitterEffect.transform.localPosition;
                }
                attractedObjects.Remove(other.transform);
            }
        }

        private bool IsAttractable(Collider other)
        {
            return other.CompareTag("Electron") || other.CompareTag("Anti-Electron");
        }
    }
}
