using System;
using System.Linq;
using GWS.Input.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GWS.Aiming.Runtime
{
    /// <summary>
    /// Manages a list of aim targets
    /// </summary>
    public class AimTargetSelector: MonoBehaviour
    {
        [SerializeField] 
        private InputEventChannel inputEventChannel;

        [SerializeField] 
        private AimTargetEventChannel aimTargetEventChannel;
        
        private const int MaxContacts = 20;

        [SerializeField, Min(0)] 
        private float contactRadius;

        [SerializeField] 
        private LayerMask contactLayerMask;
        
        private Collider[] contacts;
        
        private int contactsCount;

        private void Awake()
        {
            contacts = new Collider[20];
        }

        private void OnEnable()
        {
            inputEventChannel.OnFire2 += HandleOnFire2;
        }
        
        private void OnDisable()
        {
            inputEventChannel.OnFire2 -= HandleOnFire2;
        }

        private void HandleOnFire2(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.canceled) Unfocus();
            else FocusNearestContact();
        }

        private void FocusNearestContact()
        {
            var nearestContact = contacts
                .Take(contactsCount)
                .Where(contact => contact.gameObject != gameObject)
                .OrderBy(contact => Vector3.Distance(transform.position, contact.transform.position))
                .FirstOrDefault();
            
            if (!nearestContact) return;
            aimTargetEventChannel.RaiseOnSetLockOnTarget(nearestContact.transform);
        }

        private void Unfocus()
        {
            aimTargetEventChannel.RaiseOnUnsetLockOnTarget();
        }

        private void FixedUpdate()
        {
            contactsCount = Physics.OverlapSphereNonAlloc(transform.position, contactRadius, contacts, contactLayerMask.value);
        }
    }
}