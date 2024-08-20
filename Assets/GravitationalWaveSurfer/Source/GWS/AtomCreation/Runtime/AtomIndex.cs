using UnityEngine;

namespace GWS.AtomCreation.Runtime
{
    [CreateAssetMenu(fileName = "ElementData", menuName = "ScriptableObjects/ElementData")]
    public class AtomIndex : ScriptableObject
    {
        public int CurrentAtomIndex = 0;
    }
}