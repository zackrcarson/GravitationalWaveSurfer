using Eflatun.SceneReference;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// This logic should be in the atom creation assembly - Matthew
// using GWS.AtomCreation;

namespace GWS.SceneManagement.Runtime
{
    public class AdditiveSceneManager : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private SceneReference mainScene;
        
        private static string mainSceneName;

        /// <summary>
        /// Emits false when main scene is changed to this additive (atom) scene, emits true when the additive scene changes back to the main scene. Used to control
        /// other parts of the game (player shouldn't move when atom creation is open, etc).
        /// </summary>
        public static event Action<bool> OnChangeOfScene;

        private void Start()
        {
            OnChangeOfScene?.Invoke(false);
            mainSceneName = SceneManager.GetActiveScene().name;
            SetActiveSceneToOther();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                FadeToLevel();
            }
        }

        public void FadeToLevel()
        {
            animator.SetTrigger(MainSceneManager.FadeOutTrigger);
        }

        public void OnFadeComplete()
        {
            StartCoroutine(ReturnToMainScene());
        }
        private IEnumerator ReturnToMainScene()
        {
            SetActiveSceneToMain();
            OnChangeOfScene?.Invoke(true);
            yield return SceneManager.UnloadSceneAsync(gameObject.scene);
        }
        private void SetActiveSceneToOther()
        {
            // Set main scene to the atom scene so all instantiated objects stay within here
            SceneManager.SetActiveScene(gameObject.scene);
        }

        private void SetActiveSceneToMain()
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(mainSceneName));
        }
    }
}
