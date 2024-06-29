using System.Collections.Generic;
using UnityEngine;

namespace GWS.Player
{
    /// <summary>
    /// Attracts hydrogen bobs and moves them towards the bob of the player.
    /// </summary>
    public class BobCollection : MonoBehaviour
    {
        public static List<Transform> attractedObjects = new List<Transform>();

        private float initialSpeed = 1f;
        private float maxSpeed = 5f;
        private float accelerationRate = 0.5f; 

        void FixedUpdate()
        {
            for (int i = 0; i < attractedObjects.Count; i++)
            {
                var obj = attractedObjects[i];

                if (obj == null) continue;
                float currentSpeed = Mathf.Clamp(initialSpeed + accelerationRate * Time.time, initialSpeed, maxSpeed);
                float speed = currentSpeed * Time.deltaTime;

                obj.position = Vector3.MoveTowards(obj.position, transform.position, speed);
                if (obj.TryGetComponent<JitterEffect>(out var jitterEffect))
                {
                    jitterEffect.targetPosition = transform.position;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Electron"))
            {
                attractedObjects.Add(other.transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Electron"))
            {
                if (other.TryGetComponent<JitterEffect>(out var jitterEffect))
                {
                    jitterEffect.originalPosition = jitterEffect.transform.localPosition;
                }
                attractedObjects.Remove(other.transform);
            }
        }
    }
}
