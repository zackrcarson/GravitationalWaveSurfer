using GWS.Player.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainSceneManager : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SceneField sceneToLoad;

    [SerializeField]
    public GameObject[] hideableObjects;

    private static readonly int FADE_OUT_TRIGGER = Animator.StringToHash("FadeOut");
    private static readonly int FADE_IN_TRIGGER = Animator.StringToHash("FadeIn");

    public static event Action OnReturnToMainScene;
    private void OnEnable()
    {
        OnReturnToMainScene += ReEnableAllObjects;
    }
    private void OnDisable()
    {
        OnReturnToMainScene -= ReEnableAllObjects;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            print("G");
            FadeToLevel();
        }
    }
    public void FadeToLevel()
    {
        animator.SetTrigger(FADE_OUT_TRIGGER);
    }
    public void OnFadeComplete()
    {
        //animator.ResetTrigger(FADE_OUT_TRIGGER);
        LoadAdditiveScene();
    }
    private void LoadAdditiveScene()
    {
        HideAllObjects();
        SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        PauseGame.isInMainScene = false;
    }
    private void HideAllObjects()
    {
        foreach (GameObject obj in hideableObjects)
        {
            obj.SetActive(false);
        }
    }
    private void ReEnableAllObjects()
    {
        animator.ResetTrigger(FADE_OUT_TRIGGER);
        foreach (GameObject obj in hideableObjects)
        {
            obj.SetActive(true);
        }
        animator.SetTrigger(FADE_IN_TRIGGER);
    }
    public static void TriggerReturnToMainScene()
    {
        OnReturnToMainScene?.Invoke();
    }
}