using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWS.UI.Runtime
{
    public class ShowGlossary : MonoBehaviour
    {
        [SerializeField]
        private GameObject glossaryMenu;

        [SerializeField]
        private GameObject baseMenu;

        [SerializeField]
        private Animator glossaryAnimator;

        [SerializeField]
        private Animator baseAnimator;

        public void ShowMenu()
        {
            StartCoroutine(ShowMenuCoroutine());
        }

        private IEnumerator ShowMenuCoroutine()
        {
            glossaryMenu.SetActive(true);
            glossaryAnimator.SetTrigger("Open");
            baseAnimator.SetTrigger("Close");

            yield return new WaitForSecondsRealtime(1f / 5f);

            glossaryMenu.SetActive(true);
            baseMenu.SetActive(false);
        }

        public void HideMenu()
        {
            StartCoroutine(HideMenuCoroutine());
        }

        private IEnumerator HideMenuCoroutine()
        {
            glossaryAnimator.SetTrigger("Close");
            baseMenu.SetActive(true);
            baseAnimator.SetTrigger("Open");

            yield return new WaitForSecondsRealtime(1f / 5f);

            glossaryMenu.SetActive(false);
        }
    }

}