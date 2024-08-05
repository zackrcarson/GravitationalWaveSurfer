using GWS.Input.Runtime;
using UnityEngine;

namespace GWS.UI.Runtime
{
    /// <summary>
    /// Makes a UI element follow the mouse
    /// </summary>
    public class UIFollowMouse: MonoBehaviour
    {
        [SerializeField] 
        private InputEventChannel inputEventChannel;

        [SerializeField] 
        private RectTransform rectTransform;

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
            rectTransform.position = position;
        }
    }
}