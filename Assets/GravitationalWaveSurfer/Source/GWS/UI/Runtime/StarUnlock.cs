using UnityEngine;
using StarOutcome = GWS.UI.Runtime.Outcome.Star;

namespace GWS.UI.Runtime
{
    [CreateAssetMenu(fileName = nameof(StarUnlock), menuName = "ScriptableObjects/Unlocks/Stars")]

    public class StarUnlock : ScriptableObject, IUnlock
    {
        public new StarOutcome name;

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