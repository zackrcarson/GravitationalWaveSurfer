using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GWS.AtomCreation.Runtime
{
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Instance { get; private set; }

        [SerializeField] private Animator animator;
        [SerializeField] private SceneField mainScene;
        [SerializeField] private SceneField additiveScene;
        [SerializeField] private GameObject[] hideableObjects;

        private static readonly int FADE_OUT_TRIGGER = Animator.StringToHash("FadeOut");
        private static readonly int FADE_IN_TRIGGER = Animator.StringToHash("FadeIn");

        private string mainSceneName;
        private bool isInMainScene = true;

        public static event Action OnReturnToMainScene;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            mainSceneName = SceneManager.GetActiveScene().name;
        }

        private void OnEnable()
        {
            OnReturnToMainScene += HandleReturnToMainScene;
        }

        private void OnDisable()
        {
            OnReturnToMainScene -= HandleReturnToMainScene;
        }

        public void TransitionToAdditiveScene()
        {
            StartCoroutine(TransitionToAdditiveSceneCoroutine());
        }

        private IEnumerator TransitionToAdditiveSceneCoroutine()
        {
            yield return StartCoroutine(FadeOut());
            HideAllObjects();
            yield return SceneManager.LoadSceneAsync(additiveScene, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(additiveScene));
            isInMainScene = false;
        }

        public void TransitionToMainScene()
        {
            StartCoroutine(TransitionToMainSceneCoroutine());
        }

        private IEnumerator TransitionToMainSceneCoroutine()
        {
            yield return StartCoroutine(FadeOut());
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(mainSceneName));
            OnReturnToMainScene?.Invoke();
            isInMainScene = true;
            yield return SceneManager.UnloadSceneAsync(additiveScene);
            yield return StartCoroutine(FadeIn());
        }

        private IEnumerator FadeOut()
        {
            animator.SetTrigger(FADE_OUT_TRIGGER);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }

        private IEnumerator FadeIn()
        {
            animator.SetTrigger(FADE_IN_TRIGGER);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }

        private void HideAllObjects()
        {
            foreach (GameObject obj in hideableObjects)
            {
                obj.SetActive(false);
            }
        }

        private void HandleReturnToMainScene()
        {
            foreach (GameObject obj in hideableObjects)
            {
                obj.SetActive(true);
            }
        }

        // Optional: Add this method if you still want to trigger transitions with a key press
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.G))
            {
                if (isInMainScene)
                    TransitionToAdditiveScene();
                else
                    TransitionToMainScene();
            }
        }
    }
}