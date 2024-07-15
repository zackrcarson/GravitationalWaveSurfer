using GWS.GeneralRelativitySimulation.Runtime;
using UnityEngine;

namespace GWS.Player.Runtime
{
    public class SaucerRotation: MonoBehaviour
    {
        [SerializeField]
        private float rotationalSpeed;

        private RotationalBehavior rotationalBehavior;

        private void Start()
        {
            rotationalBehavior = GetComponent<RotationalBehavior>();
        }

        private void OnEnable()
        {
            PlayerSpeedManager.OnSpeedChanged += HandleRotation;
        }

        private void OnDisable()
        {
            PlayerSpeedManager.OnSpeedChanged -= HandleRotation;
        }

        void HandleRotation(float magnitude)
        {
            rotationalBehavior.rotationDelta = new Vector3(0, magnitude * rotationalSpeed, 0);
        }
    }
}