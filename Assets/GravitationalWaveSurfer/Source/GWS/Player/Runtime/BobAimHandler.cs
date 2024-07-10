using System;
using Cysharp.Threading.Tasks;
using GWS.Input.Runtime;
using UnityEngine;

namespace GWS.Player.Runtime
{
    /// <summary>
    /// Handles shooting the bob atop the player.
    /// </summary>
    public class BobAimHandler: MonoBehaviour
    {
        [SerializeField]
        private InputEventChannel inputEventChannel;

        [SerializeField] 
        private new Camera camera;

        [SerializeField] 
        private ConfigurableJoint bobJoint;

        [SerializeField, Min(0)] 
        private float springReactivationDelay;
        
        [SerializeField] 
        private Rigidbody bobRigidbody;
        
        /// <summary>
        /// Radius bob must be in to be fired.
        /// </summary>
        [SerializeField, Min(0)] 
        private float bobMaxActionableDistance;

        [SerializeField] 
        private float aimDepth;

        [SerializeField] 
        private float force; 

        private Vector3 cursorWorldPosition;

        private void OnEnable()
        {
            inputEventChannel.OnCursorPosition += HandleCursorPosition;
            inputEventChannel.OnFire += Fire;
        }

        private void OnDisable()
        {
            inputEventChannel.OnCursorPosition -= HandleCursorPosition; 
            inputEventChannel.OnFire -= Fire;
        }

        private void HandleCursorPosition(Vector2 position)
        {
            var currentDepth = Vector3.Distance(camera.transform.position, transform.position);
            var positionWithDepth = new Vector3(position.x, position.y, currentDepth + aimDepth);
            cursorWorldPosition = camera.ScreenToWorldPoint(positionWithDepth); 
        }   

        private void Fire()
        {
            var bobPosition = bobJoint.transform.position;
            if (Vector3.Distance(transform.position, bobPosition) > bobMaxActionableDistance) return;
            
            var direction = Vector3.Normalize(cursorWorldPosition - bobPosition);
            bobRigidbody.velocity = direction * force;
            
            bobJoint.xMotion = ConfigurableJointMotion.Free;
            bobJoint.yMotion = ConfigurableJointMotion.Free;
            bobJoint.zMotion = ConfigurableJointMotion.Free;

            DelayReactivateSpring();
        }

        private async void DelayReactivateSpring()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(springReactivationDelay), ignoreTimeScale: false);
            
            bobJoint.xMotion = ConfigurableJointMotion.Limited;
            bobJoint.yMotion = ConfigurableJointMotion.Limited;
            bobJoint.zMotion = ConfigurableJointMotion.Limited;
        }
    }
}