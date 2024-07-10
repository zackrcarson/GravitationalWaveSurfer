using GWS.UI.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GWS.UI.Runtime
{
    [CreateAssetMenu(fileName = nameof(ElementUnlock), menuName = "ScriptableObjects/Unlocks/Elements")]
    public class ElementUnlock : ScriptableObject, IUnlock
    {
        public new Outcome name;

        public Sprite sprite;

        public string description;

        public bool unlocked;

        public bool IsUnlocked()
        {
            return unlocked;
        }
    }

}