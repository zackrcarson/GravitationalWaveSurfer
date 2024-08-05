using UnityEngine;

namespace GWS.BobAiming.Runtime
{
    /// <summary>
    /// Draws a line through a spring joint, assuming the anchor points are in local space. 
    /// </summary>
    public class DrawSpringJoint: MonoBehaviour
    {
        [SerializeField]
        private Transform jointConnectedBody;

        [SerializeField] 
        private LineRenderer lineRenderer;

        private void Start()
        {
            lineRenderer.positionCount = 2; 
        }

        private void Update()
        {
            lineRenderer.SetPosition(0, jointConnectedBody.transform.position);
            lineRenderer.SetPosition(1, transform.position);
        }
    }

}