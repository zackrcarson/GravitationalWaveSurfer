using GWS.CommandPattern.Runtime;
using GWS.Input.Runtime;
using Unity.Cinemachine;
using UnityEngine;

namespace GWS.Player.Runtime
{
    public class PlayerCommandSetActiveCameraHandler: MonoBehaviour
    {
        [SerializeField]
        private InputEventChannel inputEventChannel;

        [SerializeField] 
        private CinemachineVirtualCameraBase[] cameras;

        [SerializeField] 
        private int basePriority;

        private int activeIndex; 
        
        private void OnEnable()
        {
            inputEventChannel.OnInteract += HandleOnInteract;
        }
        
        private void OnDisable()
        {
            inputEventChannel.OnInteract -= HandleOnInteract;
        }

        private void HandleOnInteract()
        {
            CommandInvoker.Execute(new PlayerCommandSetActiveCamera(cameras, activeIndex, basePriority));
            activeIndex = (activeIndex + 1) % cameras.Length;
        }
    }
}