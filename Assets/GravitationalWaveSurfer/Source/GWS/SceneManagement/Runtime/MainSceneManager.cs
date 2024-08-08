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

    public static readonly int FadeOutTrigger = Animator.StringToHash("FadeOut");
    private static readonly int FadeInTrigger = Animator.StringToHash("FadeIn");

    private void OnEnable()
    {
        AdditiveSceneManager.OnChangeOfScene += ReloadMainScene;
    }
    private void OnDisable()
    {
        AdditiveSceneManager.OnChangeOfScene -= ReloadMainScene;
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
        animator.SetTrigger(FadeOutTrigger);
    }
    public void OnFadeComplete()
    {
        //animator.ResetTrigger(FADE_OUT_TRIGGER);
        LoadAdditiveScene();
    }
    private void LoadAdditiveScene()
    {
        EnableObjects(false);
        SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
    }
    
    private void EnableObjects(bool state)
    {
        foreach (GameObject obj in hideableObjects)
        {
            obj.SetActive(state);
        }
    }

    private void ReloadMainScene(bool state)
    {
        if (!state) return;

        animator.ResetTrigger(FadeOutTrigger);
        EnableObjects(state);
        animator.SetTrigger(FadeInTrigger);
    }
}