using UnityEngine;

namespace GWS.BobAiming.Runtime
{
    /// <summary>
    /// Disables a <see cref="Collider"/> when it leaves a certain distance (and vice versa) if the <see cref="Joint"/> has a connected body.
    /// </summary>
    public class SpringJointColliderActivationRange: MonoBehaviour
    {
        [SerializeField] 
        private Joint joint;
        
        [SerializeField] 
        private new Collider collider;

        [SerializeField] 
        private Transform origin;

        [SerializeField] 
        private float maxDistance;

        public void Update()
        {
            if (joint.connectedBody == null) return;
            collider.enabled = Vector3.Distance(collider.transform.position, origin.position) <= maxDistance;
        }
    }
}