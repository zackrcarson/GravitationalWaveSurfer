using System.Collections;
using UnityEngine;
using GWS.GeneralRelativitySimulation.Runtime;
using System;

namespace GWS.Player.Runtime
{
    /// <summary>
    /// Solely emits an event whenever player speed changes.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerSpeedManager: MonoBehaviour
    {
        public static event Action<float> OnSpeedChanged;

        [SerializeField]
        private new Rigidbody rigidbody;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            OnSpeedChanged?.Invoke(rigidbody.velocity.magnitude);
        }
    }

}
