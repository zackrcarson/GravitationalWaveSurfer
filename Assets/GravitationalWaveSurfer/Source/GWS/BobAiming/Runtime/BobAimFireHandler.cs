using System;
using Cysharp.Threading.Tasks;
using GWS.Input.Runtime;
using UnityEngine;

namespace GWS.BobAiming.Runtime
{
    /// <summary>
    /// Handles shooting the bob atop the player.
    /// </summary>
    public class BobAimFireHandler: MonoBehaviour
    {
        [SerializeField]
        private InputEventChannel inputEventChannel;

        [SerializeField] 
        private Aiming.Runtime.AimData aimData;

        [SerializeField] 
        private ConfigurableJoint bobJoint;

        [SerializeField] 
        private Rigidbody connectedBody;

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
        private float force; 

        private void OnEnable()
        {
            inputEventChannel.OnFire += Fire;
        }

        private void OnDisable()
        {
            inputEventChannel.OnFire -= Fire;
        }

        private void Fire()
        {
            var bobPosition = bobJoint.transform.position;
            
            if (bobJoint.xMotion != ConfigurableJointMotion.Limited || !CanFire(transform.position, bobPosition, bobMaxActionableDistance)) return;
            
            var direction = ScreenPointToDirection(aimData.position, aimData.camera, bobPosition);
            
            bobJoint.connectedBody = null;
            bobJoint.xMotion = ConfigurableJointMotion.Free;
            bobJoint.yMotion = ConfigurableJointMotion.Free;
            bobJoint.zMotion = ConfigurableJointMotion.Free;
            
            bobRigidbody.velocity = Vector3.zero;
            bobRigidbody.AddForce(direction * force, ForceMode.Impulse);

            DelayReactivateSpring();
        }   

        private static bool CanFire(Vector3 position, Vector3 bobPosition, float maxActionableDistance)
        {
            return Vector3.Distance(position, bobPosition) <= maxActionableDistance;
        }

        private static Vector3 ScreenPointToDirection(Vector3 point, Camera camera, Vector3 position)
        {
            var distanceToCamera = Vector3.Distance(camera.transform.position, position);
            point.z += distanceToCamera;
            var pointToWorld = camera.ScreenToWorldPoint(point);
            return Vector3.Normalize(pointToWorld - position);
        }

        private async void DelayReactivateSpring()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(springReactivationDelay), ignoreTimeScale: false);
            
            bobJoint.connectedBody = connectedBody;
            bobJoint.xMotion = ConfigurableJointMotion.Limited;
            bobJoint.yMotion = ConfigurableJointMotion.Limited;
            bobJoint.zMotion = ConfigurableJointMotion.Limited;
            
            bobRigidbody.velocity = Vector3.zero;
        }
    }
}