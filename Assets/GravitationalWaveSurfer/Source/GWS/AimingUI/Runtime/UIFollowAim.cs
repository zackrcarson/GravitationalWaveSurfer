using GWS.Aiming.Runtime;
using UnityEngine;

namespace GWS.AimingUI.Runtime
{
    /// <summary>
    /// Makes a UI element follow the mouse
    /// </summary>
    public class UIFollowAim: MonoBehaviour
    {
        [SerializeField] 
        private AimData aimData;

        [SerializeField] 
        private RectTransform rectTransform;

        [SerializeField, Range(0, 1)] 
        private float interpolation;

        private void LateUpdate()
        {
            rectTransform.position = Vector3.Lerp(rectTransform.position, aimData.position, interpolation);
        }
    }
}