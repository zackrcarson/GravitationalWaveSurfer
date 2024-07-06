using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GWS.GeneralRelativitySimulation.Runtime;

public class InGameMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject glossaryMenu;

    [SerializeField]
    private Animator pauseMenuAnimator;

    [SerializeField]
    private Animator glossaryMenuAnimator;

    [SerializeField]
    private GameObject statsMenu;

    private bool isPaused = false;

    private float currentTimeScale = 1f;

    private void Start()
    {
        pauseMenuAnimator = pauseMenu.GetComponent<Animator>();
        glossaryMenuAnimator = glossaryMenu.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !glossaryMenu.activeSelf)
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

        if (Input.GetKeyDown(KeyCode.Escape) && !pauseMenu.activeSelf && glossaryMenu.activeSelf)
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
        TimeSpeedManager.Scale = 1f;
        isPaused = true;
        AudioListener.volume = 1;
        pauseMenuAnimator.SetTrigger("Open");

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
        pauseMenuAnimator.SetTrigger("Close");

        yield return new WaitForSecondsRealtime(1f / 5f);

        pauseMenu.SetActive(false);
        statsMenu.SetActive(true);
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
        glossaryMenuAnimator.SetTrigger("Open");
        pauseMenuAnimator.SetTrigger("Close");

        yield return new WaitForSecondsRealtime(1f / 5f);

        glossaryMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void HideGlossary()
    {
        StartCoroutine(HideGlossaryRoutine());
    }

    private IEnumerator HideGlossaryRoutine()
    {
        glossaryMenuAnimator.SetTrigger("Close");
        pauseMenu.SetActive(true);
        pauseMenuAnimator.SetTrigger("Open");

        yield return new WaitForSecondsRealtime(1f / 5f);

        glossaryMenu.SetActive(false);
    }
}
