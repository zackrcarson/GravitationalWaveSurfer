using Eflatun.SceneReference;
using GWS.CommandPattern.Runtime;
using UnityEngine.SceneManagement;

namespace GWS.SceneManagement.Runtime
{
    /// <summary>
    /// Loads a scene. 
    /// </summary>
    public readonly struct LoadSceneCommand: ICommand
    {
        private readonly SceneReference sceneReference;

        public LoadSceneCommand(SceneReference sceneReference) : this()
        {
            this.sceneReference = sceneReference;
        }

        public void Execute()
        {
            if (sceneReference.UnsafeReason != SceneReferenceUnsafeReason.None) return;
            SceneManager.LoadScene(sceneReference.Name);
        }
    }
}