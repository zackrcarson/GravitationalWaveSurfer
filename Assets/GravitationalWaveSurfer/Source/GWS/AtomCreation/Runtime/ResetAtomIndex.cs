using UnityEngine;

namespace GWS.AtomCreation.Runtime
{
    public class ResetAtomIndex : MonoBehaviour
    {
        [SerializeField] private AtomIndex formationIndex;
        void Start()
        {
            formationIndex.CurrentAtomIndex = 0;
        }
    }
}
