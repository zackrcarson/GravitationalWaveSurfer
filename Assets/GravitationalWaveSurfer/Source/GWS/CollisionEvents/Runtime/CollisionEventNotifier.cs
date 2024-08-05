using UnityEngine;

namespace GWS.CollisionEvents.Runtime
{
    /// <summary>
    /// Raises events from a <see cref="CollisionEventChannel"/> based on its own collision events so
    /// that they're subscribable from any arbitrary class. 
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CollisionEventNotifier: MonoBehaviour
    {
        [SerializeField] 
        private CollisionEventChannel collisionEventChannel;

        [SerializeField] 
        private LayerMask includeLayerMask;

        private void OnTriggerEnter(Collider other)
        {
            if (!IsIncludedInLayerMask(other.gameObject, includeLayerMask.value)) return;
            collisionEventChannel.RaiseOnTriggerEnter(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!IsIncludedInLayerMask(other.gameObject, includeLayerMask.value)) return;
            collisionEventChannel.RaiseOnTriggerStay(other);
        }


        private void OnTriggerExit(Collider other)
        {
            if (!IsIncludedInLayerMask(other.gameObject, includeLayerMask.value)) return;
            collisionEventChannel.RaiseOnTriggerExit(other);
        }
        
        private static bool IsIncludedInLayerMask(GameObject gameObject, int layerMask)
        {
            return (layerMask & (1 << gameObject.layer)) > 0;
        }
    }
}