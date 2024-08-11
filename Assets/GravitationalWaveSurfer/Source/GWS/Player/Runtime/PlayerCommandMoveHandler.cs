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

        /// <summary>
        /// Target axis of movement for player movement driven by the rotation.
        /// </summary>
        [SerializeField] 
        private Transform referenceFrame;

        [SerializeField, Min(0)]
        private float referenceFrameInterpolationSpeed; 

        /// <summary>
        /// Current axis of movement for player movement that interpolates towards the <see cref="referenceFrame"/>.
        /// </summary>
        private Quaternion currentReferenceFrame; 
        
        private Vector3 direction;

        public static bool canMove = true;

        private void OnEnable()
        {
            inputEventChannel.OnMove += HandleOnMove;
            AdditiveSceneManager.OnChangeOfScene += HandleSceneChange;
        }
        
        private void OnDisable()
        {
            inputEventChannel.OnMove -= HandleOnMove;
            AdditiveSceneManager.OnChangeOfScene -= HandleSceneChange;
        }

        private void HandleSceneChange(bool state)
        {
            print($"CALLED {state}");
            canMove = state;
        }

        private void HandleOnMove(Vector2 value)
        {
            direction = new Vector3(value.x, 0, value.y);
        }

        private void Start()
        {
            currentReferenceFrame = referenceFrame.rotation;
        }

        private void FixedUpdate()
        {
            if (!canMove) return;

            currentReferenceFrame = Quaternion.Slerp(currentReferenceFrame, referenceFrame.rotation, referenceFrameInterpolationSpeed * Time.deltaTime);
            var rotation = Quaternion.Euler(0, currentReferenceFrame.eulerAngles.y, 0);
            CommandInvoker.Execute(new RigidbodyCommandMove(rigidbody, direction * speed, rotation, friction));

            // TODO - move to new input system
            if (UnityEngine.Input.GetKey(KeyCode.Space))
            {
                CommandInvoker.Execute(new RigidbodyCommandMove(rigidbody, new Vector3(0, 1, 0), rotation, friction));
            }
            else if (UnityEngine.Input.GetKey(KeyCode.Q))
            {
                CommandInvoker.Execute(new RigidbodyCommandMove(rigidbody, new Vector3(0, -1, 0), rotation, friction));
            }
        }
    }
    
}