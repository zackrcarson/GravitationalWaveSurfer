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

        private bool enableEmission;

        private Vector3 movementDirection;
        
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

        private void OnEnable()
        {
            inputEventChannel.OnMove += HandleMovement;
        }
        
        private void OnDisable()
        {
            inputEventChannel.OnMove -= HandleMovement;
        }

        private void Start()
        {
            enableEmission = false;
        }

        private void Update()
        {
            HandleThrusterParticles(forwardThruster, maxParticles, movementDirection, -forwardThruster.transform.up, enableEmission);
            HandleThrusterParticles(backThruster, maxParticles, movementDirection, -backThruster.transform.up, enableEmission);
            HandleThrusterParticles(leftThruster, maxParticles, movementDirection, -leftThruster.transform.up, enableEmission);
            HandleThrusterParticles(rightThruster, maxParticles, movementDirection, -rightThruster.transform.up, enableEmission);
        }
        
        private void HandleMovement(Vector2 movementValue)
        {
            enableEmission = movementValue.sqrMagnitude > 0;
            movementDirection = new Vector3(movementValue.x, 0, movementValue.y);
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
