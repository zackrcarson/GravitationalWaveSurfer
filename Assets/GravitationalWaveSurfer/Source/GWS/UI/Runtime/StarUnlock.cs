using UnityEngine;

namespace GWS.UI.Runtime
{
    [CreateAssetMenu(fileName = nameof(StarUnlock), menuName = "ScriptableObjects/Unlocks/Stars")]

    public class StarUnlock : ScriptableObject, IUnlock
    {
        public new Outcome Name;

        public Sprite Sprite;

        public string Description;

        public double SolarMass;

        public bool Unlocked;

        public bool IsUnlocked()
        {
            return Unlocked;
        }
    }

}