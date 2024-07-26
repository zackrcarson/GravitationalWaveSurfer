using GWS.Input.Runtime;
using UnityEngine;

namespace GWS.Aiming.Runtime
{
    /// <summary>
    /// Controls the aim position based on the cursor position.
    /// </summary>
    public class AimFollowCursor: MonoBehaviour
    {
        [SerializeField]
        private InputEventChannel inputEventChannel;

        [SerializeField] 
        private AimData aimData;
        
        /// <summary>
        /// The z-axis depth of the aim.
        /// </summary>
        [SerializeField] 
        private float zDepth;
        
        private void OnEnable()
        {
            inputEventChannel.OnCursorPosition += HandleCursorPosition;
        }
        
        private void OnDisable()
        {
            inputEventChannel.OnCursorPosition -= HandleCursorPosition;
        }
        
        private void HandleCursorPosition(Vector2 position)
        {
            aimData.position = new Vector3(position.x, position.y, zDepth);
        }
    }
}