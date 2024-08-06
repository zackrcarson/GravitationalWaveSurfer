using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GWS.Player.Runtime;
public class AdditiveSceneManager : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SceneField mainScene;

    private static readonly int FADE_OUT_TRIGGER = Animator.StringToHash("FadeOut");
    private static string mainSceneName;
   
    private void Start()
    {
        //animator.SetTrigger(FADE_IN_TRIGGER);
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
        animator.SetTrigger(FADE_OUT_TRIGGER);
    }
    public void OnFadeComplete()
    {
        StartCoroutine(ReturnToMainScene());
    }
    private IEnumerator ReturnToMainScene()
    {
        SetActiveSceneToMain();
        MainSceneManager.TriggerReturnToMainScene();
        PauseGame.isInMainScene = true;
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