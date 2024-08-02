using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GWS.UI.Runtime
{
    public class TypewriterEffect : MonoBehaviour
    {
        [SerializeReference]
        private TextMeshProUGUI text;

        [SerializeField]
        private float timeBetweenCharacters;

        public void ShowText()
        {
            StartCoroutine(ShowTextCoroutine());
        }

        private IEnumerator ShowTextCoroutine()
        {
            int totalCharacters = text.textInfo.characterCount;

            for (int counter = 0; counter <= totalCharacters; counter++)
            {
                text.ForceMeshUpdate();
                text.maxVisibleCharacters = counter;
                yield return new WaitForSeconds(timeBetweenCharacters);
            }
        }
    }

}