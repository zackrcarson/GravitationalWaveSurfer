using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace GWS.Aiming.Runtime
{
    /// <summary>
    /// Manages a list of aim targets
    /// </summary>
    public class AimTargetEvaluator: MonoBehaviour
    {
        [SerializeField] 
        private AimData aimData;

        /// <summary>
        /// The offset of the capsule in its forward direction.
        /// </summary>
        [SerializeField] 
        private float capsuleDepth;

        [SerializeField]
        private float capsuleRadius;

        [SerializeField]
        private float capsuleHeight; 

        [SerializeField]
        private LayerMask includeLayerMask;
        
        private Collider[] contacts;
        
        private const int MaxContacts = 20;

        private bool shouldUpdateLockOnTarget; 

        private void Awake()
        {
            contacts = new Collider[MaxContacts];
        }

        public void HandleOnCameraActivated(ICinemachineMixer _, ICinemachineCamera __)
        {
            shouldUpdateLockOnTarget = false;
        }

        public void HandleOnCameraDeactivated(ICinemachineMixer _, ICinemachineCamera __)
        {
            shouldUpdateLockOnTarget = true;
        }

        private void FixedUpdate()
        {
            if (!shouldUpdateLockOnTarget) return;
            var contactsCount = GetContacts(contacts);
            var nearestCollider = GetNearestCollider(contacts.Take(contactsCount));
            aimData.LockOnTarget = nearestCollider ? nearestCollider.transform : null;
        }

        private int GetContacts(Collider[] results)
        {
            var position = aimData.camera.transform.position;
            var direction = AimDataManager.ScreenPointToDirection(aimData.position, aimData.camera, position);
            var point0 = position + direction * capsuleDepth;
            var point1 = point0 + direction * capsuleHeight;
            return Physics.OverlapCapsuleNonAlloc(point0, point1, capsuleRadius, results, includeLayerMask.value);
        }
        
        private Collider GetNearestCollider(IEnumerable<Collider> colliders)
        {
            return colliders
                .Where(contact => contact.gameObject != gameObject)
                .OrderBy(contact => Vector3.Distance(transform.position, contact.transform.position))
                .FirstOrDefault();
        }
    }
}