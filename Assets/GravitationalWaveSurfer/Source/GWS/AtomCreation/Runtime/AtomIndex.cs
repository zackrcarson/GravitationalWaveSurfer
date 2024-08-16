using UnityEngine;

namespace GWS.AtomCreation
{
    [CreateAssetMenu(fileName = "ElementData", menuName = "ScriptableObjects/ElementData")]
    public class AtomIndex : ScriptableObject
    {
        public int CurrentAtomIndex = 0;
    }
}