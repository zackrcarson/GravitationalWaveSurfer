using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;

namespace GWS.UI.Runtime
{
    public class DisplayStarUnlock : DisplayBaseUnlock
    {
        [SerializeField]
        protected Image image;

        [SerializeField]
        protected new TextMeshProUGUI name;

        [SerializeField]
        protected TextMeshProUGUI description;

        [SerializeField] 
        private StarUnlock star;

        [SerializeField]
        private TextMeshProUGUI solarMass;

        protected override bool IsUnlocked() => star.IsUnlocked();

        protected override void PopulateFields()
        {
            // Adds a space between camel case names, i.e. BlackHole -> Black Hole
            name.text = Regex.Replace(star.name.ToString(), "(?<!^)([A-Z])", " $1");
            solarMass.text = $"{star.solarMass} M›";
            image.sprite = star.sprite;
            description.text = star.description;
        }

        protected override void SetElements(bool state)
        {
            base.SetElements(state);
            name.enabled = state;
            solarMass.enabled = state;
            image.enabled = state;
            description.enabled = state;
        }
    }
}
