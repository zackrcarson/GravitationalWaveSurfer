using GWS.UI.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GlossaryPopulator : MonoBehaviour
{
    public GameObject elementEntryPrefab;
    public GameObject elementParent;

    [ContextMenu("Instantiate Atom Entries")]
    public void InstantiateAtomEntries()
    {
        // Path to the folder containing the AtomUnlock ScriptableObjects
        string folderPath = "Assets/GravitationalWaveSurfer/ScriptableObjects/Unlocks/Elements/Atoms";

        // Get all AtomUnlock ScriptableObjects
        string[] guids = AssetDatabase.FindAssets("t:AtomUnlock", new[] { folderPath });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            AtomUnlock atomUnlock = AssetDatabase.LoadAssetAtPath<AtomUnlock>(assetPath);

            if (atomUnlock != null)
            {
                GameObject elementEntry = PrefabUtility.InstantiatePrefab(elementEntryPrefab, elementParent.transform) as GameObject;
                if (elementEntry != null)
                {
                    DisplayElementUnlock displayElementUnlock = elementEntry.GetComponent<DisplayElementUnlock>();
                    if (displayElementUnlock != null)
                    {
                        displayElementUnlock.element = atomUnlock;
                        displayElementUnlock.PopulateFields();

                        // Rename the instantiated object to match the atom name
                        elementEntry.name = $"ElementEntry_{atomUnlock.FullName}";
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
        }

        // Save the changes to the scene
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

}
