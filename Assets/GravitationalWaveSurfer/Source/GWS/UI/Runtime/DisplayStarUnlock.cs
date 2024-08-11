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

        public override void PopulateFields()
        {
            // Adds a space between camel case names, i.e. BlackHole -> Black Hole
            name.text = Regex.Replace(star.Name.ToString(), "(?<!^)([A-Z])", " $1");
            solarMass.text = $"{star.SolarMass} M›";
            image.sprite = star.Sprite;
            description.text = star.Description;
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
