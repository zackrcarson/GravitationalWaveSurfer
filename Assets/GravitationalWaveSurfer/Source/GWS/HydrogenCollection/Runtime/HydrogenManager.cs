using System;
using UnityEngine;

namespace GWS.HydrogenCollection.Runtime
{
    /// <summary>
    /// HydrogenEater.cs Instance gets created multiple times for some reason <br/>
    /// This manager is guaranteed to only exist by itself so multiplier works properly <br/>
    /// Right now AddHydrogen() has some redundancy but doesn't affect much so keeping it this way. 
    /// </summary>
    public class HydrogenManager : MonoBehaviour
    {
        public static HydrogenManager Instance { get; private set; }

        [Header("Multiplier")]
        public int multiplier;

        private void Awake() 
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);    
        }

        public void ChangeMultiplier(int value)
        {
            multiplier = value;
        }

        public void AddHydrogen(int value)
        {
            int amount = (int) (value * Math.Pow(10, multiplier));
            HydrogenEater.Instance.AddHydrogen(amount);
        }
    }
}