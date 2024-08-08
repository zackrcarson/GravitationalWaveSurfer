using GWS.UI.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AtomScriptableObjectsGenerator : MonoBehaviour
{
    [MenuItem("Tools/Generate Atom Unlocks")]
    public static void GenerateAtomUnlocks()
    {
        var atoms = AtomInfo.AllAtoms;

        foreach (var atom in atoms)
        {
            AtomUnlock atomUnlock = ScriptableObject.CreateInstance<AtomUnlock>();
            atomUnlock.Atom = atom.Atom;
            atomUnlock.FullName = atom.FullName;
            atomUnlock.Protons = atom.Protons;
            atomUnlock.Neutrons = atom.Neutrons;
            atomUnlock.Electrons = atom.Electrons;
            atomUnlock.Mass = atom.Mass;
            atomUnlock.Description = "Undefined.";
            atomUnlock.Unlocked = false;

            string path = $"Assets/GravitationalWaveSurfer/ScriptableObjects/Unlocks/Elements/Atoms/{atom.FullName}.asset";
            AssetDatabase.CreateAsset(atomUnlock, path);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Atom Unlocks generated successfully!");
    }
}
