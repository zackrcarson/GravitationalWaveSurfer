using GWS.UI.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GWS.UI.Runtime
{
    [CreateAssetMenu(fileName = nameof(StarUnlock), menuName = "ScriptableObjects/Unlocks/Stars")]
    public class StarUnlock : ScriptableObject, IUnlock
    {
        public new string name;

        public Sprite sprite;

        public string description;

        public double solarMass;

        public bool unlocked;

        public bool IsUnlocked()
        {
            return unlocked;
        }
    }

}