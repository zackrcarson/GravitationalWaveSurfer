using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GWS.UI.Runtime
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject glossaryMenu;
        [SerializeField] private Animator glossaryMenuAnimator;
        
        private void Start()
        {
            if (glossaryMenuAnimator == null) glossaryMenuAnimator = glossaryMenu.GetComponent<Animator>();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && glossaryMenu.activeSelf)
            {
                HideGlossary();
            }
        }

        public void ShowGlossary()
        {
            StartCoroutine(ShowGlossaryCoroutine());
        }

        public void HideGlossary()
        {
            StartCoroutine(HideGlossaryCoroutine());
        }

        private IEnumerator ShowGlossaryCoroutine()
        {
            glossaryMenu.SetActive(true);
            glossaryMenuAnimator.SetTrigger("Open");
            yield return new WaitForSecondsRealtime(0.2f);
        }

        private IEnumerator HideGlossaryCoroutine()
        {
            glossaryMenuAnimator.SetTrigger("Close");
            yield return new WaitForSecondsRealtime(0.2f);
            glossaryMenu.SetActive(false);
        }

        public void PlayGame()
        {
            SceneManager.LoadScene("FinalBuild");
        }

        public void QuitGame()
        {
            Debug.Log("QUIT");
            Application.Quit();
        }
    }
}
