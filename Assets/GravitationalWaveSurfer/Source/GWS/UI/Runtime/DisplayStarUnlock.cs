using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GWS.UI.Runtime
{
    public class DisplayStarUnlock : DisplayBaseUnlock
    {
        [SerializeField] 
        private StarUnlock star;

        [SerializeField] 
        private TextMeshProUGUI starName;

        [SerializeField]
        private TextMeshProUGUI solarMass;

        protected override bool IsUnlocked() => star.IsUnlocked();

        protected override void PopulateFields()
        {
            starName.text = star.name;
            solarMass.text = $"{star.solarMass} MÅõ";
            image.sprite = star.sprite;
            description.text = star.description;
        }

        protected override void SetElements(bool state)
        {
            base.SetElements(state);
            starName.enabled = state;
            solarMass.enabled = state;
        }
    }
}
