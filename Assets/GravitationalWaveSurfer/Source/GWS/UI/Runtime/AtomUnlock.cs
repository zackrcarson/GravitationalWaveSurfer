using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWS.UI.Runtime
{
    [CreateAssetMenu(fileName = nameof(ElementUnlock), menuName = "ScriptableObjects/Unlocks/Atom")]
    public class AtomUnlock : ScriptableObject, IUnlock
    {
        public Outcome Atom;
        public string FullName;
        public int Protons;
        public int Neutrons;
        public int Electrons;
        public double Mass;

        public string Description = "Undefined.";

        public bool Unlocked;

        public bool IsUnlocked()
        {
            return Unlocked;
        }
    }

}