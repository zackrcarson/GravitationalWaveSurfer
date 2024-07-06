using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GWS.UI.Runtime
{
    public class DisplayElementUnlock : MonoBehaviour
    {
        [SerializeField]
        private ElementUnlock element;

        [SerializeField]
        private TextMeshProUGUI chemicalSymbol;

        [SerializeField]
        private TextMeshProUGUI superscript;

        [SerializeField]
        private TextMeshProUGUI subscript;

        [SerializeField]
        private Image image;

        [SerializeField]
        private TextMeshProUGUI description;

        [SerializeField]
        private TextMeshProUGUI lockSymbol;

        private void OnEnable()
        {
            DisplayElement();
        }

        private void DisplayElement()
        {
            if (!element.IsUnlocked())
            {
                lockSymbol.enabled = true;
                SetElements(false);
                return;
            }

            SetElements(true);
            lockSymbol.enabled = false;

            chemicalSymbol.text = element.chemicalSymbol;
            superscript.text = element.superscript.ToString();
            subscript.text = element.subscript.ToString();
            image.sprite = element.sprite;
            description.text = element.description;
        }

        private void SetElements(bool state)
        {
            chemicalSymbol.enabled = state;
            superscript.enabled = state;
            subscript.enabled = state;
            image.enabled = state;
            description.enabled = state;
        }
    }
}
