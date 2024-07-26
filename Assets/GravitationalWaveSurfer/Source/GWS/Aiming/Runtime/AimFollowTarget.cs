using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GWS.Aiming.Runtime
{
    /// <summary>
    /// Manages the screen-space aim position.
    /// </summary>
    public class AimFollowTarget: MonoBehaviour
    {
        [SerializeField] 
        private AimData aimData;
        
        public Transform aimTarget;
        
        private void Start()
        {
            aimTarget = null;
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2f, Screen.height / 2f));
        }

        private void OnDisable() => Start();

        private void Update()
        {
            if (!aimTarget) return;
            aimData.position = aimData.camera.WorldToScreenPoint(aimTarget.position);
        }
    }
}