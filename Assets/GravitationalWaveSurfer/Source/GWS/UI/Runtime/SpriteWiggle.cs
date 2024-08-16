using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWS.UI.Runtime
{
    public class SpriteWiggle : MonoBehaviour
    {
        [SerializeField] private Vector3 startRotation;
        [SerializeField] private Vector3 endRotation;
        [SerializeField] private float lerpSpeed = 1f;

        private float t = 0f;
        private bool isForward = true;

        private void Update()
        {
            if (isForward)
            {
                t += Time.deltaTime * lerpSpeed;
                if (t >= 1f)
                {
                    t = 1f;
                    isForward = false;
                }
            }
            else
            {
                t -= Time.deltaTime * lerpSpeed;
                if (t <= 0f)
                {
                    t = 0f;
                    isForward = true;
                }
            }

            transform.rotation = Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(endRotation), t);
        }
    }
}
