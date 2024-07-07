using GWS.Input.Runtime;
using UnityEngine;

namespace GWS.Player.Runtime
{
    public class BobAimHandler: MonoBehaviour
    {
        [SerializeField]
        private InputEventChannel inputEventChannel;

        [SerializeField] 
        private Transform bobOrigin;

        [SerializeField] 
        private new Camera camera;

        [SerializeField] 
        private float cursorZOffset; 

        private Vector3 cursorWorldPosition;

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
            var offsetPosition = new Vector3(position.x, position.y, cursorZOffset);
            cursorWorldPosition = camera.ScreenToWorldPoint(offsetPosition); 
        }

        private void Update()
        {
            Debug.Log((Vector2)cursorWorldPosition);
            var position = bobOrigin.position;
            Debug.DrawLine(position, position + Vector3.Normalize(cursorWorldPosition - position) * 100, Color.white, Time.deltaTime);
        }
    }
}