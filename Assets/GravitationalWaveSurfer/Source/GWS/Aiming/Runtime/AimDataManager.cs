using UnityEngine;

namespace GWS.Aiming.Runtime
{
    /// <summary>
    /// Initializes <see cref="AimData"/>
    /// </summary>
    public class AimDataManager: MonoBehaviour
    {
        [SerializeField] 
        private AimData aimData;

        [SerializeField] 
        private new Camera camera;

        private void Awake()
        {
            aimData.camera = camera;
        }
        
        /// <summary>
        /// Converts a screen point to a normalized direction vector in world space.
        /// </summary>
        /// <param name="point">The screen point to convert.</param>
        /// <param name="camera">The camera through which the screen point is viewed.</param>
        /// <param name="position">The reference position in world space from which the direction is calculated.</param>
        /// <returns>A normalized direction vector from the reference position to the screen point in world space.</returns>
        public static Vector3 ScreenPointToDirection(Vector3 point, Camera camera, Vector3 position)
        {
            var distanceToCamera = Vector3.Distance(camera.transform.position, position);
            point.z += distanceToCamera;
            var pointToWorld = camera.ScreenToWorldPoint(point);
            return Vector3.Normalize(pointToWorld - position);
        }
    }
}