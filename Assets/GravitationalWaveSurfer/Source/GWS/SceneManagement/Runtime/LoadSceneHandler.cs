using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement.Runtime
{
    /// <summary>
    /// Loads a scene. 
    /// </summary>
    public class LoadSceneHandler: MonoBehaviour
    {
        public SceneReference scene;

        public void Load()
        {
            if (scene.UnsafeReason != SceneReferenceUnsafeReason.None) return;
            SceneManager.LoadScene(scene.Name);
        }
    }
}