using GWS.UI.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWS.AtomCreation.Runtime
{
    public static class ElementInfo 
    {
        //public static ElementData Hydrogen = new ElementData { Element = Outcome.H, MeV = 0, lowerThreshold = 0.2, upperThreshold = 0.8f, duration = 100 };

        public static readonly ElementData[] Order =
        {
            new ElementData { Element = Outcome.H2, MeV = 1.112f, lowerThreshold = 0.4f, upperThreshold = 0.9f, duration = 100 },
            new ElementData { Element = Outcome.He3, MeV = 2.573f, lowerThreshold = 0.4f, upperThreshold = 0.8f, duration = 100 },
            new ElementData { Element = Outcome.He4, MeV = 7.074f, lowerThreshold = 0.5f, upperThreshold = 0.8f, duration = 100 },
            new ElementData { Element = Outcome.C, MeV = 7.680f, lowerThreshold = 0.2f, upperThreshold = 0.8f, duration = 100 },
            new ElementData { Element = Outcome.O, MeV = 7.976f, lowerThreshold = 0.2f, upperThreshold = 0.8f, duration = 100 },
            new ElementData { Element = Outcome.Ne, MeV = 8.032f, lowerThreshold = 0.2f, upperThreshold = 0.8f, duration = 100 },
            new ElementData { Element = Outcome.S, MeV = 8.332f, lowerThreshold = 0.2f, upperThreshold = 0.8f, duration = 100 },
            new ElementData { Element = Outcome.Fe, MeV = 8.790f, lowerThreshold = 0.2f, upperThreshold = 0.8f, duration = 100 },
        };
    }

    public struct ElementData
    {
        public Outcome Element { get; set; }
        public float MeV { get; set; }
        public float lowerThreshold { get; set; }
        public float upperThreshold { get; set; }
        public int duration { get; set; }
    }
}
