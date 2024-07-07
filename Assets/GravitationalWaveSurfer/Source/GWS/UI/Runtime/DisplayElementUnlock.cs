using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GWS.UI.Runtime
{
    public class DisplayElementUnlock : DisplayBaseUnlock
    {
        [SerializeField]
        private ElementUnlock element;

        [SerializeField]
        private TextMeshProUGUI chemicalSymbol;

        [SerializeField]
        private TextMeshProUGUI superscript;

        [SerializeField]
        private TextMeshProUGUI subscript;

        protected override bool IsUnlocked() => element.IsUnlocked();

        protected override void PopulateFields()
        {
            chemicalSymbol.text = element.chemicalSymbol;
            superscript.text = element.superscript.ToString();
            subscript.text = element.subscript.ToString();
            image.sprite = element.sprite;
            description.text = element.description;
        }

        protected override void SetElements(bool state)
        {
            base.SetElements(state);
            chemicalSymbol.enabled = state;
            superscript.enabled = state;
            subscript.enabled = state;
        }
    }
}
