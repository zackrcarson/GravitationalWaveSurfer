using System.Collections;
using GWS.GeneralRelativitySimulation.Runtime;
using UnityEngine;

namespace GWS.Player.Runtime
{
    public class PauseGame : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;

        [SerializeField] private Animator animator;

        [SerializeField]
        private GameObject glossaryMenu;

        [SerializeField]
        private Animator pauseMenuAnimator;

        [SerializeField]
        private Animator glossaryMenuAnimator;

        [SerializeField] private GameObject statsMenu;

        [SerializeField, Min(0)] 
        private float pauseTime = 0.25f; 

        private bool isPaused = false;

        private float currentTimeScale = 1f;

        public static bool isInMainScene = true;
        
        private static readonly int Open = Animator.StringToHash("Open");
        private static readonly int Close = Animator.StringToHash("Close");

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && !glossaryMenu.activeSelf)
            {
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    StopGame();
                }
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && glossaryMenu.activeSelf)
            {
                HideGlossary();
            }
        }

        public void StopGame()
        {
            StartCoroutine(StopGameCoroutine());
        }

        private IEnumerator StopGameCoroutine()
        {
            pauseMenu.SetActive(true);
            statsMenu.SetActive(false);
            currentTimeScale = Time.timeScale;
            TimeSpeedManager.Scale = 0f;
            isPaused = true;
            AudioListener.volume = 1;
            animator.SetTrigger(Open);

            yield return new WaitForSecondsRealtime(pauseTime);

            pauseMenu.SetActive(true);
            statsMenu.SetActive(false);
        }

        public void ResumeGame()
        {
            StartCoroutine(ResumeGameCoroutine());
        }

        private IEnumerator ResumeGameCoroutine()
        {
            animator.SetTrigger(Close);

            yield return new WaitForSecondsRealtime(pauseTime);

            pauseMenu.SetActive(false);
            if (isInMainScene)
            {
                statsMenu.SetActive(true);
            }
            TimeSpeedManager.Scale = currentTimeScale;
            isPaused = false;
            AudioListener.volume = 1;
        }

        public void ShowGlossary()
        {
            StartCoroutine(ShowGlossaryCoroutine());
        }

        private IEnumerator ShowGlossaryCoroutine()
        {
            glossaryMenu.SetActive(true);
            glossaryMenuAnimator.SetTrigger(Open);
            pauseMenuAnimator.SetTrigger(Close);

            yield return new WaitForSecondsRealtime(pauseTime);

            glossaryMenu.SetActive(true);
            pauseMenu.SetActive(false);
        }

        public void HideGlossary()
        {
            StartCoroutine(HideGlossaryRoutine());
        }

        private IEnumerator HideGlossaryRoutine()
        {
            glossaryMenuAnimator.SetTrigger(Close);
            pauseMenu.SetActive(true);
            pauseMenuAnimator.SetTrigger(Open);

            yield return new WaitForSecondsRealtime(pauseTime);

            glossaryMenu.SetActive(false);
        }
    }
}
