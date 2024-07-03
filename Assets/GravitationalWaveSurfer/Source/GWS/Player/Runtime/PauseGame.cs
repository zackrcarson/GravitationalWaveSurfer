using System.Collections;
using GWS.GeneralRelativitySimulation.Runtime;
using UnityEngine;

namespace GWS.Player.Runtime
{
    public class PauseGame : MonoBehaviour
    {
        [SerializeField]
        private GameObject pauseMenu;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private GameObject statsMenu;

        private bool isPaused = false;

        private float currentTimeScale = 1f;

        private void Start()
        {
        
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
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
            AudioListener.volume = 0;
            animator.SetTrigger("Open");

            yield return new WaitForSecondsRealtime(1f / 5f);

            pauseMenu.SetActive(true);
            statsMenu.SetActive(false);
        }

        public void ResumeGame()
        {
            StartCoroutine(ResumeGameCoroutine());
        }

        private IEnumerator ResumeGameCoroutine()
        {
            animator.SetTrigger("Close");

            yield return new WaitForSecondsRealtime(1f / 5f);

            pauseMenu.SetActive(false);
            statsMenu.SetActive(true);
            TimeSpeedManager.Scale = currentTimeScale;
            isPaused = false;
            AudioListener.volume = 1;
        }
    }
}
