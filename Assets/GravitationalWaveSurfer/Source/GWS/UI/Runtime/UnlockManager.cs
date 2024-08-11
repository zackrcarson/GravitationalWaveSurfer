using UnityEngine;
using System.Collections.Generic;
using GWS.UI.Runtime;
using System;
using System.Linq;
using UnityEditor;

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
        private List<AtomUnlock> allElementUnlocks;

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

        [ContextMenu("Add Atom Prefabs")]
        public void AddAtomPrefabs()
        {
            const string FolderPath = "Assets/GravitationalWaveSurfer/ScriptableObjects/Unlocks/Elements/Atoms";
            string[] guids = AssetDatabase.FindAssets("t:AtomUnlock", new[] { FolderPath });

            List<AtomUnlock> atomUnlocks = new List<AtomUnlock>();

            // https://discussions.unity.com/t/local-filepath-for-unity-asset/908990
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                AtomUnlock atomUnlock = AssetDatabase.LoadAssetAtPath<AtomUnlock>(assetPath);
                if (atomUnlock != null)
                {
                    atomUnlocks.Add(atomUnlock);
                }
            }

            // Sort the AtomUnlock objects by proton count
            atomUnlocks = atomUnlocks.OrderBy(a => a.Protons).ToList();

            foreach (AtomUnlock atomUnlock in atomUnlocks)
            {
                allElementUnlocks.Add(atomUnlock);
            }

            // This saves the changes to the scene.
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }

        public void UnlockOutcome(Outcome outcome)
        {
            if (!currentUnlocks.Contains(outcome))
            {
                currentUnlocks.Add(outcome);
                SaveUnlocks();
                LoadUnlocks();
                OnUnlock?.Invoke();
                //print(PlayerPrefs.GetString(UNLOCKED_OUTCOMES_STRING, ""));
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
                outcomeUnlock.Unlocked = savedOutcomes.Contains(outcomeUnlock.Name.ToString());
            }

            foreach (AtomUnlock outcomeUnlock in allElementUnlocks)
            {
                outcomeUnlock.Unlocked = savedOutcomes.Contains(outcomeUnlock.Atom.ToString());
            }
        }

        public void ResetUnlocks()
        {
            PlayerPrefs.DeleteAll();
            currentUnlocks.Clear();
            foreach (StarUnlock outcomeUnlock in allStarUnlocks)
            {
                outcomeUnlock.Unlocked = false;
            }

            foreach (AtomUnlock outcomeUnlock in allElementUnlocks)
            {
                outcomeUnlock.Unlocked = false;
            }
            OnUnlock?.Invoke();
        }
    }
}