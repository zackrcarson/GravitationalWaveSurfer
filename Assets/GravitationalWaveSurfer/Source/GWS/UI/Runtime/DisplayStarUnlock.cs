using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GWS.UI.Runtime
{
    public class DisplayStarUnlock : MonoBehaviour
    {
        [SerializeField]
        private StarUnlock star;

        [SerializeField]
        private new TextMeshProUGUI name;

        [SerializeField]
        private TextMeshProUGUI solarMass;

        [SerializeField]
        private Image image;

        [SerializeField]
        private TextMeshProUGUI description;

        [SerializeField]
        private TextMeshProUGUI lockSymbol;

        private void OnEnable()
        {
            DisplayStar();
        }

        private void DisplayStar()
        {
            if (!star.IsUnlocked())
            {
                lockSymbol.enabled = true;
                SetElements(false);
                return;
            }

            SetElements(true);
            lockSymbol.enabled = false;

            name.text = star.name;
            solarMass.text = $"{star.solarMass} MÅõ";
            image.sprite = star.sprite;
            description.text = star.description;
        }

        private void SetElements(bool state)
        {
            name.enabled = state;
            solarMass.enabled = state;
            image.enabled = state;
            description.enabled = state;
        }
    }
}
