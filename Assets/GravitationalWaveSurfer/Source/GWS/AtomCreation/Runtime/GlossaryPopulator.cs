using GWS.UI.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GlossaryPopulator : MonoBehaviour
{
    public GameObject elementEntryPrefab;
    public GameObject elementParent;

    [ContextMenu("Instantiate Atom Entries")]
    public void InstantiateAtomEntries()
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
            GameObject elementEntry = PrefabUtility.InstantiatePrefab(elementEntryPrefab, elementParent.transform) as GameObject;
            if (elementEntry != null)
            {
                DisplayElementUnlock displayElementUnlock = elementEntry.GetComponent<DisplayElementUnlock>();
                if (displayElementUnlock != null)
                {
                    displayElementUnlock.element = atomUnlock;
                    displayElementUnlock.PopulateFields();
                    elementEntry.name = $"{atomUnlock.FullName}Entry";
                }
                else
                {
                    Debug.LogError($"DisplayElementUnlock component not found on prefab for {atomUnlock.FullName}");
                }
            }
            else
            {
                Debug.LogError($"Failed to instantiate prefab for {atomUnlock.FullName}");
            }
        }

        // This saves the changes to the scene.
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }


}
