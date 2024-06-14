using UnityEngine;

namespace GWS.GeneralRelativitySimulation.Runtime
{
    public class RotationalBehavior: MonoBehaviour
    {
        [SerializeField] private Vector3 rotationDelta;

        private void Update()
        {
            Rotate();
        }

        private void Rotate()
        {
            transform.Rotate(rotationDelta * Time.deltaTime);
        }
    }
}