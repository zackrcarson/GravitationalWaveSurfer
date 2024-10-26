using System.Threading;
using GWS.Aiming.Runtime;
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
        private AimData aimData;

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

        private CancellationTokenSource fireCancellationToken = new();

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
            fireCancellationToken.Cancel();
            fireCancellationToken.Dispose();
            fireCancellationToken = new CancellationTokenSource();
            
            var bobPosition = bobJoint.transform.position;
            if (bobJoint.xMotion != ConfigurableJointMotion.Limited || !IsCloseEnough(transform.position, bobPosition, bobMaxActionableDistance)) return;
            
            var direction = AimDataManager.ScreenPointToDirection(aimData.position, aimData.camera, bobPosition);
            bobJoint.connectedBody = null;
            SetXYZMotion(bobJoint, ConfigurableJointMotion.Free);
            bobRigidbody.velocity = Vector3.zero;
            bobRigidbody.AddForce(direction * force, ForceMode.Impulse);

            DelayReactivateSpring(fireCancellationToken.Token);
        }   

        private static bool IsCloseEnough(Vector3 position, Vector3 bobPosition, float maxActionableDistance)
        {
            return Vector3.Distance(position, bobPosition) <= maxActionableDistance;
        }

        private static void SetXYZMotion(ConfigurableJoint joint, ConfigurableJointMotion motion)
        {
            joint.xMotion = motion;
            joint.yMotion = motion;
            joint.zMotion = motion;
        }

        private async void DelayReactivateSpring(CancellationToken cancellationToken)
        {
            //throws error
            await Awaitable.WaitForSecondsAsync(springReactivationDelay, cancellationToken);
            
            bobJoint.connectedBody = connectedBody;
            SetXYZMotion(bobJoint, ConfigurableJointMotion.Limited);
            
            bobRigidbody.velocity = Vector3.zero;
        }
    }
}