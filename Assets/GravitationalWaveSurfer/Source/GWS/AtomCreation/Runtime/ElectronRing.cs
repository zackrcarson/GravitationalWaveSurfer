using System.Collections.Generic;
using UnityEngine;

namespace GWS.AtomCreation.Runtime
{    
    public class ElectronRing : MonoBehaviour 
    {
        public int maxElectron = 0;
        public int numElectron = 0;
        public int rotationSign;
        public bool active = false;
        public bool full = false;
        public GameObject selfReference; // might not be necessary
        public List<GameObject> electronReferences;
        public List<Vector3> electronPositions;

        private void Start() {
            rotationSign = (UnityEngine.Random.Range(0f, 1f) > 0.5f) ? 1 : -1;
        }
        public void setMaxElectron(int n) {maxElectron = n;}
        public void incNumElectron() {numElectron++;}
        public void decNumElectron() {numElectron--;}
        public void toggleFull() {full = !full;}
        public void toggleActive() {active = !active;}

    }
}