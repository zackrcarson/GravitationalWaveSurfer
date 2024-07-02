using UnityEngine;
using GWS.Input.Runtime;

namespace GWS.Player.Runtime
{
    /// <summary>
    /// Handles the particles and rotation of player thrusters.
    /// </summary>
    public class ThrusterVisualsHandler: MonoBehaviour
    {
        [SerializeField] 
        private InputEventChannel inputEventChannel;
        
        /// <summary>
        /// Determines the direction that the thrusters should be moving.
        /// </summary>
        /// <remarks>Avoids being hardcoded to anything, and more accurately reflects the player movement direction.</remarks>
        [SerializeField] 
        private Rigidbody referenceRigidbody;

        [SerializeField]
        private bool enableEmission;
        
        /// <summary>
        /// The number used to clamp emission rate of particles.
        /// </summary>
        [SerializeField]
        private float maxParticles;

        [SerializeField]
        private ParticleSystem forwardThruster;

        [SerializeField]
        private ParticleSystem leftThruster;

        [SerializeField]
        private ParticleSystem backThruster;

        [SerializeField]
        private ParticleSystem rightThruster;

        // [SerializeField]
        // private float rotationLerp;
        
        // [SerializeField]
        // private float rotationMaxSpeed;

        // private KeyCode[] keys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
        //
        // private Dictionary<KeyCode, GameObject> keyMappings;

        /// <summary>
        /// The angle which the particles will emit, based off player input.
        /// </summary>
        //private Quaternion targetRotation = Quaternion.Euler(new Vector3(90, 0, 0));

        private void OnEnable()
        {
            inputEventChannel.OnMove += EnableEmission;
        }
        
        private void OnDisable()
        {
            inputEventChannel.OnMove -= EnableEmission;
        }

        private void Start()
        {
            // keyMappings = new()
            // {
            //     { KeyCode.W, topThruster },
            //     { KeyCode.A, leftThruster },
            //     { KeyCode.S, bottomThruster },
            //     { KeyCode.D, rightThruster },
            // };

            enableEmission = false;
        }

        private void Update()
        {
            // keys.ToList().ForEach(key =>
            // {
            //     HandleInputParticles(maxParticles, keyMappings[key].GetComponentInChildren<ParticleSystem>(), key);
            //     HandleThrusterSpin(keyMappings[key].GetComponentInChildren<RotationalBehavior>(), key);
            // });  

            var velocity = referenceRigidbody.velocity;
            var scaledMovementDirection = Vector3.Normalize(velocity); 
            HandleThrusterParticles(forwardThruster, maxParticles, scaledMovementDirection, -forwardThruster.transform.up, enableEmission);
            HandleThrusterParticles(backThruster, maxParticles, scaledMovementDirection, -backThruster.transform.up, enableEmission);
            HandleThrusterParticles(leftThruster, maxParticles, scaledMovementDirection, -leftThruster.transform.up, enableEmission);
            HandleThrusterParticles(rightThruster, maxParticles, scaledMovementDirection, -rightThruster.transform.up, enableEmission);
        }

        // private void HandleThrusterSpin(RotationalBehavior thrusterRotation, KeyCode key)
        // {
        //     float target = UnityEngine.Input.GetKey(key) ? rotationMaxSpeed : 0f;
        //     thrusterRotation.rotationDelta.y = Mathf.Lerp(thrusterRotation.rotationDelta.y, target, Time.deltaTime * rotationLerp);
        // }

        private void EnableEmission(Vector2 movementValue)
        {
            enableEmission = movementValue.sqrMagnitude > 0;
        } 

        private static void HandleThrusterParticles(ParticleSystem particles, float maxEmissionRate, Vector3 movementDirection, Vector3 thrusterDirection, bool enableEmission)
        {
            var emission = particles.emission;

            var projection = Vector3.Dot(movementDirection, thrusterDirection);
            if (enableEmission && projection > 0f)
            {
                emission.enabled = true;
                emission.rateOverTime = projection * maxEmissionRate;
            }
            else
            {
                emission.enabled = false;
            }
        }
    }

}
