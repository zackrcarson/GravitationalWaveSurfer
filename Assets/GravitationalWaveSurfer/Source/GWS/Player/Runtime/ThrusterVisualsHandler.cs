using UnityEngine;
using GWS.Input.Runtime;
using System.Linq;
using System.Collections.Generic;
using Codice.CM.Client.Differences;
using GWS.GeneralRelativitySimulation.Runtime;

namespace GWS.Player.Runtime
{
    /// <summary>
    /// Handles the particles and rotation of player thrusters.
    /// </summary>
    public class ThrusterVisualsHandler: MonoBehaviour
    {
        /// <summary>
        /// The number used to clamp emission rate of particles.
        /// </summary>
        [SerializeField]
        private float maxParticles;

        [SerializeField]
        private GameObject topThruster;

        [SerializeField]
        private GameObject leftThruster;

        [SerializeField]
        private GameObject bottomThruster;

        [SerializeField]
        private GameObject rightThruster;

        [SerializeField]
        private float rotationLerp;

        [SerializeField]
        private float rotationMaxSpeed;

        private KeyCode[] keys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };

        private Dictionary<KeyCode, GameObject> keyMappings;

        /// <summary>
        /// The angle which the particles will emit, based off player input.
        /// </summary>
        //private Quaternion targetRotation = Quaternion.Euler(new Vector3(90, 0, 0));

        private void Start()
        {
            keyMappings = new()
            {
                { KeyCode.W, topThruster },
                { KeyCode.A, leftThruster },
                { KeyCode.S, bottomThruster },
                { KeyCode.D, rightThruster },
            };
        }

        private void Update()
        {
            keys.ToList().ForEach(key =>
            {
                HandleInputParticles(maxParticles, keyMappings[key].GetComponentInChildren<ParticleSystem>(), key);
                HandleThrusterSpin(keyMappings[key].GetComponentInChildren<RotationalBehavior>(), key);
            });
        }

        private void HandleThrusterSpin(RotationalBehavior thrusterRotation, KeyCode key)
        {
            float target = UnityEngine.Input.GetKey(key) ? rotationMaxSpeed : 0f;
            thrusterRotation.rotationDelta.y = Mathf.Lerp(thrusterRotation.rotationDelta.y, target, Time.deltaTime * rotationLerp);
        }

        private void HandleInputParticles(float rate, ParticleSystem particles, KeyCode key)
        {
            var emission = particles.emission;

            if (UnityEngine.Input.GetKey(key))
            {
                emission.enabled = true;
                emission.rateOverTime = Mathf.Clamp(rate, 0, maxParticles);
            }
            else
            {
                emission.enabled = false;
            }
        }
    }

}
