using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GWS.UI.Runtime
{
    public class DisplayElementUnlock : DisplayBaseUnlock
    {
        [SerializeField] public AtomUnlock element;

        [SerializeField] private GameObject elementBox;

        [SerializeField] private Image elementBoxImage;
        [SerializeField] private TextMeshProUGUI atomicNumber;
        [SerializeField] private TextMeshProUGUI atomicSymbol;
        [SerializeField] private TextMeshProUGUI elementName;
        [SerializeField] private TextMeshProUGUI atomicMass;

        public static readonly Color WeirdOrange = new Color(255f / 255f, 186f / 255f, 181f / 255f);

        public static readonly Outcome[] NuclearFusionOrder = { Outcome.H, Outcome.H2, Outcome.He3, Outcome.He4, Outcome.C, Outcome.O, Outcome.Fe };

        private void Start()
        {
           
        }

        protected override bool IsUnlocked() => element.IsUnlocked();

        public override void PopulateFields()
        {
            atomicNumber.text = element.Protons.ToString();
            atomicSymbol.text = element.Atom.ToString();
            elementName.text = element.FullName;
            atomicMass.text = element.Mass.ToString();

            if (NuclearFusionOrder.Contains(element.Atom))
            {
                elementBoxImage.color = WeirdOrange;
            }
            else
            {
                elementBoxImage.color = Color.white;
            }
        }

        protected override void SetElements(bool state)
        {
            base.SetElements(state);
            atomicNumber.enabled = state;
            atomicSymbol.enabled = state;
            elementName.enabled = state;
            atomicMass.enabled = state;
        }
    }
}
