using UnityEngine;

namespace GWS.UI.Runtime
{
    [CreateAssetMenu(fileName = nameof(StarUnlock), menuName = "ScriptableObjects/Unlocks/Stars")]

    public class StarUnlock : ScriptableObject, IUnlock
    {
        public new Outcome name;

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