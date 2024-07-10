using System.Collections.Generic;
using GWS.HydrogenCollection.Runtime;
using UnityEngine;

namespace GWS.Player.Runtime
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

        void FixedUpdate()
        {
            foreach (var obj in attractedObjects)
            {
                if (obj == null) continue;

                obj.position = Vector3.MoveTowards(obj.position, transform.position, 0.09f);
                if (obj.TryGetComponent<JitterEffect>(out var jitterEffect))
                {
                    jitterEffect.targetPosition = transform.position;
                }
            }
    
            collider.radius = colliderRadiusMultiplier * particleInventory.HydrogenCount + baseRadius;
            float appropriateScale = collider.radius * 2;
            radiusVisual.transform.localScale = Vector3.one * appropriateScale;
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
