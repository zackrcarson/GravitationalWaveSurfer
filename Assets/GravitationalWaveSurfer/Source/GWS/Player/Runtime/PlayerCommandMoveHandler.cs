using GWS.Character.Runtime;
using GWS.CommandPattern.Runtime;
using GWS.Input.Runtime;
using UnityEngine;
using UnityEngine.Serialization;

namespace GWS.Player.Runtime
{
    /// <summary>
    /// Handles the player movement.
    /// </summary>
    public class PlayerCommandMoveHandler: MonoBehaviour
    {
        [SerializeField]
        private InputEventChannel inputEventChannel;
        
        [SerializeField]
        private new Rigidbody rigidbody;

        [SerializeField]
        private float speed;
        
        [SerializeField, Range(0f, 1f)]
        private float friction;

        [SerializeField] 
        private Transform referenceFrame;
        
        private Vector3 direction; 
        
        private void OnEnable()
        {
            inputEventChannel.OnMove += HandleOnMove;
        }
        
        private void OnDisable()
        {
            inputEventChannel.OnMove -= HandleOnMove;
        }

        private void HandleOnMove(Vector2 value)
        {
            direction = new Vector3(value.x, 0, value.y);
        }

        private void FixedUpdate()
        {
            var rotation = Quaternion.Euler(0, referenceFrame.eulerAngles.y, 0);
            CommandInvoker.Execute(new RigidbodyCommandMove(rigidbody, direction * speed, rotation, friction)); 
        }
    }
    
}