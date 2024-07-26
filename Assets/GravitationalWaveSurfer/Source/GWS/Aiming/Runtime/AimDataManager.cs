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
    }
}