using UnityEngine;
using System.Collections.Generic;
using GWS.UI.Runtime;
using System;

namespace GWS.Gameplay
{
    public class UnlockManager : MonoBehaviour
    {
        public static UnlockManager Instance { get; private set; }

        public const string UNLOCKED_OUTCOMES_STRING = "UnlockedOutcomes";

        public static event Action OnUnlock;

        private HashSet<Outcome> currentUnlocks = new();

        [SerializeField]
        private StarUnlock[] allStarUnlocks;

        [SerializeField]
        private ElementUnlock[] allElementUnlocks;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                LoadUnlocks();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void UnlockOutcome(Outcome outcome)
        {
            if (!currentUnlocks.Contains(outcome))
            {
                currentUnlocks.Add(outcome);
                SaveUnlocks();
                LoadUnlocks();
                OnUnlock?.Invoke();
            }
        }

        public bool IsOutcomeUnlocked(Outcome outcome)
        {
            return currentUnlocks.Contains(outcome);
        }

        private void SaveUnlocks()
        {
            PlayerPrefs.SetString(UNLOCKED_OUTCOMES_STRING, string.Join(",", currentUnlocks));
            PlayerPrefs.Save();
        }

        private void LoadUnlocks()
        {
            string savedOutcomes = PlayerPrefs.GetString(UNLOCKED_OUTCOMES_STRING, "");
            if (!string.IsNullOrEmpty(savedOutcomes))
            {
                foreach (string outcome in savedOutcomes.Split(','))
                {
                    if (Enum.TryParse(outcome, out Outcome parsedOutcome))
                    {
                        currentUnlocks.Add(parsedOutcome);
                    }
                }
            }

            foreach (StarUnlock outcomeUnlock in allStarUnlocks)
            {
                outcomeUnlock.unlocked = savedOutcomes.Contains(outcomeUnlock.name.ToString());
            }

            foreach (ElementUnlock outcomeUnlock in allElementUnlocks)
            {
                outcomeUnlock.unlocked = savedOutcomes.Contains(outcomeUnlock.name.ToString());
            }
        }

        public void ResetUnlocks()
        {
            PlayerPrefs.DeleteAll();
            currentUnlocks.Clear();
            foreach (StarUnlock outcomeUnlock in allStarUnlocks)
            {
                outcomeUnlock.unlocked = false;
            }

            foreach (ElementUnlock outcomeUnlock in allElementUnlocks)
            {
                outcomeUnlock.unlocked = false;
            }
            OnUnlock?.Invoke();
        }
    }
}