using UnityEngine;
using System.Collections.Generic;
using GWS.UI.Runtime;
using StarOutcome = GWS.UI.Runtime.Outcome.Star;
using System;

namespace GWS.Gameplay
{
    public class UnlockManager : MonoBehaviour
    {
        public static UnlockManager Instance { get; private set; }

        public const string UNLOCKED_OUTCOMES_STRING = "UnlockedOutcomes";

        public static event Action<string> OnUnlock;

        private HashSet<StarOutcome> starUnlocks = new();

        [SerializeField]
        private StarUnlock[] allStarUnlocks;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadUnlocks();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void UnlockOutcome(StarOutcome outcome)
        {
            if (!starUnlocks.Contains(outcome))    
            {
                starUnlocks.Add(outcome);
                SaveUnlocks();
                LoadUnlocks();
                OnUnlock?.Invoke(nameof(outcome));
            }
        }

        public bool IsOutcomeUnlocked(StarOutcome outcome)
        {
            return starUnlocks.Contains(outcome);
        }

        private void SaveUnlocks()
        {
            PlayerPrefs.SetString(UNLOCKED_OUTCOMES_STRING, string.Join(",", starUnlocks));
            PlayerPrefs.Save();
        }

        private void LoadUnlocks()
        {
            string savedOutcomes = PlayerPrefs.GetString(UNLOCKED_OUTCOMES_STRING, "");
            if (!string.IsNullOrEmpty(savedOutcomes))
            {
                foreach (string outcome in savedOutcomes.Split(','))
                {
                    if (Enum.TryParse(outcome, out StarOutcome parsedOutcome))
                    {
                        starUnlocks.Add(parsedOutcome);
                    }
                }
            }

            foreach (StarUnlock starUnlock in allStarUnlocks)
            {
                if (savedOutcomes.Contains(starUnlock.name.ToString())) starUnlock.unlocked = true;
            }
        }

        public void ResetUnlocks()
        {
            PlayerPrefs.DeleteAll();
            starUnlocks.Clear();

            foreach (StarUnlock starUnlock in allStarUnlocks) starUnlock.unlocked = false;
            OnUnlock?.Invoke("");
        }

    }
}