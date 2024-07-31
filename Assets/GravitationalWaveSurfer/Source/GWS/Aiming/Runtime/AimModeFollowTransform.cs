using UnityEngine;
using UnityEngine.InputSystem;

namespace GWS.Aiming.Runtime
{
    /// <summary>
    /// Makes an <see cref="AimData"/> update based on a world-space target.
    /// </summary>
    public class AimModeFollowTransform: MonoBehaviour
    {
        [SerializeField] 
        private AimData aimData;
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2f, Screen.height / 2f));
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void Update()
        {
            if (!aimData.LockOnTarget) return;
            aimData.position = aimData.camera.WorldToScreenPoint(aimData.LockOnTarget.position);
        }
    }
}