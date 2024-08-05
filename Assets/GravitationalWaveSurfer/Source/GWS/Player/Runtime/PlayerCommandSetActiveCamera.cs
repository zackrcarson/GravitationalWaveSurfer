using System.Collections.Generic;
using System.Linq;
using GWS.CommandPattern.Runtime;
using Unity.Cinemachine;

namespace GWS.Player.Runtime
{
    /// <summary>
    /// Cycles through a list of Cinemachine cameras by adjusting their priorities.
    /// </summary>
    public readonly struct PlayerCommandSetActiveCamera: ICommand
    {
        private readonly IEnumerable<CinemachineVirtualCameraBase> cameras;
        private readonly int activeIndex;
        private readonly int basePriority;

        public PlayerCommandSetActiveCamera(IEnumerable<CinemachineVirtualCameraBase> cameras, int activeIndex, int basePriority)
        {
            this.cameras = cameras;
            this.activeIndex = activeIndex;
            this.basePriority = basePriority;
        }

        public void Execute()
        {
            for (var i = 0; i < cameras.Count(); i++)
            {
                cameras.ElementAt(i).Priority = basePriority + i == activeIndex ? 1 : 0;
            }
        }
    }
}