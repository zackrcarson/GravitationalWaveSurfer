using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

namespace GWS.AtomCreation.Runtime
{
    public class CreationManagement : MonoBehaviour, IPointerClickHandler
    {
        [Header("Simulation")]
        public int numElectron = 10;
        public int numProton = 10;
        public int numNeutron = 10;
        public int numElectronRings = 1;
        public int numInNucleus;
        public GameObject electronPrefab;
        public GameObject neutronPrefab;
        public GameObject protonPrefab;
        public GameObject electronRingPrefab;
        public GameObject nucleusFolder; // nucleusFolder that holds all generated particles]
        public GameObject eRingFolder;
        public string nucleusFolderName = "Nucleus";
        public string eRingFolderName = "ElectronRings";

        public float gravitationalConstant = 50f;
        public float threshold = 0.1f;
        public int maxIterations = 1000;
        public float initialVelocityRange = 1f; // Initial random velocity range
        public float spawnSphereRadius = 10f; // Radius of the spawn sphere
        public float dampingFactor = 0.95f; // Factor to manually decrease the velocity

        private List<int> eRingConfiguration = new List<int>{2, 8, 18, 32, 32, 18, 8};
        public float eRingRadius = 7f;  // Radius of first ring
        public float eRingRadiusDiff = 3f;  // Radius difference between each ring
        public float radiusChangeSpeed = 3f; // Radius change per second when necessary
        private float targetRadiusChange = 0f;  // Radius change left to be performed globally
        public float eRingAngularVelocity = 50f;

        private List<GameObject> protonParticles;
        private List<GameObject> neutronParticles;
        private List<List<GameObject>> electronParticles;
        private List<GameObject> electronRings;
        private List<Rigidbody> protonRigidbodies;
        private List<Rigidbody> neutronRigidbodies;

        public GameObject TallyCount;
        private TextMeshProUGUI numProtonUI;
        private TextMeshProUGUI numNeutronUI;
        private TextMeshProUGUI numElectronUI;
        [SerializeField] TextMeshProUGUI numAddRemoveIncrement;
        public GameObject UI;
        public bool UIVisible = true;

        private static readonly Vector3 ORIGIN_POINT = new Vector3(0, 10000, 0);

        private int addRemoveIncrement = 1;
        private int currentIncrementIndex = 0;

        public static Action<int, int, int> OnParticlesChanged;
        public static Action<string> OnWarningRaised;

        private void Start()
        {
            if (nucleusFolder == null) nucleusFolder = new GameObject(nucleusFolderName);
            nucleusFolder.transform.position = ORIGIN_POINT;
            if (eRingFolder == null) eRingFolder = new GameObject(eRingFolderName);
            eRingFolder.transform.position = ORIGIN_POINT;

            protonParticles = new List<GameObject>();
            neutronParticles = new List<GameObject>();

            electronParticles = new List<List<GameObject>>();
            electronParticles.Add(new List<GameObject>());
            electronRings = new List<GameObject>();

            protonRigidbodies = new List<Rigidbody>();
            neutronRigidbodies = new List<Rigidbody>();

            numInNucleus = numProton + numNeutron;
            eRingRadius = 7f;

            numProtonUI = TallyCount.transform.Find("Protons/NumProtons").GetComponent<TextMeshProUGUI>();
            numNeutronUI = TallyCount.transform.Find("Neutrons/NumNeutrons").GetComponent<TextMeshProUGUI>();
            numElectronUI = TallyCount.transform.Find("Electrons/NumElectrons").GetComponent<TextMeshProUGUI>();

            StartCoroutine(Simulate(true));
        }

        private void Update() 
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
            {
                // Toggle the visibility of UI elements
                UI.SetActive(!UIVisible);
                UIVisible = !UIVisible;
            }

            if (targetRadiusChange != 0f)
            {
                float radiusChangeThisFrame = radiusChangeSpeed * Time.deltaTime;

                if (Mathf.Abs(targetRadiusChange) <= radiusChangeThisFrame)
                {
                    ChangeERingRadius(targetRadiusChange);
                    targetRadiusChange = 0f;
                }
                else
                {
                    float radiusChangeDirection = Mathf.Sign(targetRadiusChange);   // Grow or shrink
                    ChangeERingRadius(radiusChangeDirection * radiusChangeThisFrame); // Apply direction to change per frame value
                    targetRadiusChange -= radiusChangeDirection * radiusChangeThisFrame;
                }
            }

            UpdateElectronRingRotation();
            UpdateTallyCount();
        }

        public void StartSimulation(bool initialize)
        {
            StartCoroutine(Simulate(initialize));
        }

        IEnumerator Simulate(bool initialize)
        {
            if (initialize) 
            {
                InitializeNucleus();
                InitializeElectronRings();
            }
            Physics.gravity = Vector3.zero; // Disable default gravity

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                bool hasConverged = true;

                // Update gravitational force for each particle in the nucleus
                foreach (Rigidbody rb in protonRigidbodies)
                {
                    // Gravity: inward direction towards origin
                    Vector3 directionToOrigin = (ORIGIN_POINT - rb.position).normalized;
                    rb.AddForce(directionToOrigin * gravitationalConstant, ForceMode.Acceleration);
                    rb.velocity *= dampingFactor;   // Manual damping
                }
                foreach (Rigidbody rb in neutronRigidbodies)
                {
                    // Gravity: inward direction towards origin
                    Vector3 directionToOrigin = (ORIGIN_POINT - rb.position).normalized;
                    rb.AddForce(directionToOrigin * gravitationalConstant, ForceMode.Acceleration);
                    // Manually take away energy to help convergence
                    rb.velocity *= dampingFactor;   // Manual damping
                }

                yield return new WaitForFixedUpdate(); // Wait for physics update

                // Check for convergence
                if (numInNucleus > 1)
                {
                    foreach (Rigidbody rb in protonRigidbodies)
                    {
                        if (rb.velocity.magnitude > threshold) hasConverged = false;
                    }
                    foreach (Rigidbody rb in neutronRigidbodies)
                    {
                        if (rb.velocity.magnitude > threshold) hasConverged = false;
                    }
                }
                else if (numInNucleus == 1)
                {
                    if (numProton == 1)
                    {
                        //Debug.Log(Vector3.Distance(protonParticles[0].transform.position, Vector3.zero));
                    }
                    if (numProton == 1 && Vector3.Distance(protonParticles[0].transform.position, Vector3.zero) > threshold)
                    {
                        hasConverged = false;
                    }
                    if (numNeutron == 1 && Vector3.Distance(neutronParticles[0].transform.position, Vector3.zero) > threshold)
                    {
                        hasConverged = false;
                    }    
                    
                    
                }

                if (hasConverged)
                {
                    //Debug.Log("Simulation has converged.");
                    StopAllMovement();
                    yield break;
                }
            }

            StopAllMovement();
            //Debug.Log("Max iterations reached without convergence.");
        }

        void InitializeNucleus()
        {
            //Debug.Log("Initializing nucleus.");

            for (int i = 0; i < numInNucleus; i++)
            {
                if (i < numNeutron)
                {
                    SpawnParticle("Neutron", i);
                }
                else
                {
                    SpawnParticle("Proton", i);
                }
            }
        }

        void InitializeElectronRings()
        {
            //Debug.Log("Initializing electron ring.");

            for (int i = 0; i < eRingConfiguration.Count; i++) 
            {
                GameObject ring = Instantiate(electronRingPrefab, ORIGIN_POINT, Quaternion.identity);
                float radius = eRingRadius + i * eRingRadiusDiff;
                ring.transform.localScale = new Vector3(radius, radius, radius);
                electronRings.Add(ring);

                ring.name = $"ElectronRing_{i}"; // rename for cleanliness
                ring.transform.parent = eRingFolder.transform; // all particles in a nucleusFolder for cleanliness

                ElectronRing eRingObject = ring.GetComponent<ElectronRing>();
                eRingObject.setMaxElectron(eRingConfiguration[i]);
                eRingObject.selfReference = ring;

                ring.SetActive(false);
            }
        }

        void UpdateERingRadius(bool remove=false)
        {
            if (numInNucleus == 0)
            {
                targetRadiusChange = eRingRadius - electronRings[0].transform.localScale.x;
                return;
            }
            if (numInNucleus == 1) return;  // Base case to avoid asymptotic behavior

            float before;
            if (remove) before = (float) Math.Floor(Math.Log(numInNucleus+1, 2));
            else before = (float) Math.Floor(Math.Log(numInNucleus-1, 2));

            float updated = (float) Math.Floor(Math.Log(numInNucleus, 2));

            if (before == updated) return;

            float dr = (updated - before) * 0.5f;
            targetRadiusChange += dr;
        }

        void ChangeERingRadius(float radiusChange)
        {
            foreach (GameObject eRing in electronRings)
            {
                eRing.transform.localScale += new Vector3(radiusChange, radiusChange, radiusChange);
            }
        }

        void UpdateElectronRingRotation()
        {
            for (int i = 0; i < electronRings.Count; i++)
            {
                GameObject eRing = electronRings[i];
                ElectronRing ring = eRing.GetComponent<ElectronRing>();
                int sign = ring.rotationSign;
                int direction = (i % 2 == 0) ? 1 : -1;

                if (ring.active)
                {
                    eRing.transform.Rotate(direction * Vector3.forward, eRingAngularVelocity * Time.deltaTime);

                    float fluctuationAngleX = Mathf.Sin(Time.time * 0.8f) * 0.02f * sign;
                    float fluctuationAngleY = Mathf.Cos(Time.time * 0.4f) * 0.02f * sign;

                    // Create a Quaternion representing the fluctuation rotations
                    Quaternion fluctuationRotation = Quaternion.Euler(fluctuationAngleX, fluctuationAngleY, 0f);

                    // Apply the fluctuation rotations to the electron ring
                    eRing.transform.localRotation *= fluctuationRotation;
                }
            }
        }

        void SpawnParticle(string type, int i)
        {
            Vector3 randomDirection = UnityEngine.Random.onUnitSphere; // Point on the surface of a unit sphere
            Vector3 spawnPosition = ORIGIN_POINT + (randomDirection * spawnSphereRadius); // Scale to the desired radius

            // Instantiate particle objects
            GameObject particle;
            if (type == "Neutron") 
            {
                particle = Instantiate(neutronPrefab, spawnPosition, Quaternion.identity);

                particle.name = $"Neutron_{i}"; // rename for cleanliness
                particle.transform.parent = nucleusFolder.transform; // all particles in a nucleusFolder for cleanliness

                neutronParticles.Add(particle);

                // Get Rigidbody
                Rigidbody rb = particle.GetComponent<Rigidbody>();
                neutronRigidbodies.Add(rb);

                // Apply an initial random velocity to kickstart convergence
                Vector3 initialVelocity = UnityEngine.Random.insideUnitSphere * initialVelocityRange;
                rb.velocity = initialVelocity;            
            }
            else if (type == "Proton") 
            {
                particle = Instantiate(protonPrefab, spawnPosition, Quaternion.identity);

                particle.name = $"Proton_{i}"; // rename for cleanliness
                particle.transform.parent = nucleusFolder.transform; // all particles in a nucleusFolder for cleanliness

                protonParticles.Add(particle);

                // Get Rigidbody
                Rigidbody rb = particle.GetComponent<Rigidbody>();
                protonRigidbodies.Add(rb);

                // Apply an initial random velocity to kickstart convergence
                Vector3 initialVelocity = UnityEngine.Random.insideUnitSphere * initialVelocityRange;
                rb.velocity = initialVelocity;
            }
            else if (type == "Electron")
            {
                // particle = Instantiate(electronPrefab, spawn)
            }
            else {
                Debug.LogWarning("Invalid type of particle!"); 
                return;
            }
        }

        /// <summary>
        /// Supports adding neutrons and protons
        /// </summary>
        /// <param name="type">"neutron" or "proton"</param>
        public void AddParticle(string type)
        {
            //Debug.Log($"Adding {type}...");
            if (type == "Proton") numProton++;
            else if (type == "Neutron") numNeutron++;
            else
            {
                Debug.LogWarning("Invalid particle type!!");
                return;
            }
            numInNucleus++;
            UpdateERingRadius();
            SpawnParticle(type, numInNucleus-1);
            StartAllMovement();
            StartSimulation(false);
        }

        /// <summary>
        /// Supports removing neutrons and protons
        /// </summary>
        /// <param name="type">"neutron" or "proton"</param>
        public void RemoveParticle(string type)
        {
            List<GameObject> particleList;
            if (type == "Neutron") particleList = neutronParticles;
            else if (type == "Proton") particleList = protonParticles;
            else
            {
                //Debug.LogWarning("Invalid type of particle!");
                return;
            }

            if (particleList.Count == 0)
            {
                EmitWarning($"No {type} particles left to remove!");
                return;
            }

            if (type == "Neutron")
            {
                int lastIndex = neutronParticles.Count - 1;

                GameObject particleToRemove = neutronParticles[lastIndex];
                Rigidbody rbToRemove = neutronRigidbodies[lastIndex];

                neutronParticles.RemoveAt(lastIndex);
                neutronRigidbodies.RemoveAt(lastIndex);

                Destroy(particleToRemove);
                Destroy(rbToRemove);

                numNeutron--;
            }
            else if (type == "Proton")
            {
                int lastIndex = protonParticles.Count - 1;

                GameObject particleToRemove = protonParticles[lastIndex];
                Rigidbody rbToRemove = protonRigidbodies[lastIndex];

                protonParticles.RemoveAt(lastIndex);
                protonRigidbodies.RemoveAt(lastIndex);

                Destroy(particleToRemove);
                Destroy(rbToRemove);

                numProton--;
            }

            numInNucleus--;

            // Debug.Log($"Removed {type}.");    
            UpdateERingRadius(true);
            StartAllMovement();
            StartSimulation(false);
        }

        public void AddElectron()
        {
            // find which electron ring to add new electron to
            int whereToAdd = -1;
            for (int i = 0; i < electronRings.Count; i++)
            {
                if (!(electronRings[i].GetComponent<ElectronRing>().full))
                {
                    whereToAdd = i;
                    break;
                }
            }

            // NO MORE SPACE! this is when all rings are filled
            if (whereToAdd == -1)
            {
                EmitWarning("No more room to add electrons!!");
                return;
            }

            // some variable declarations
            GameObject eRing = electronRings[whereToAdd];
            ElectronRing eRingData = eRing.GetComponent<ElectronRing>();
            List<Vector3> newPositions = new List<Vector3>();
            int newElectronIndex = eRingData.numElectron;

            if (!eRingData.active)
            {
                eRingData.toggleActive();
                eRing.SetActive(true);
            }

            // update numElectron in electron ring
            eRingData.incNumElectron();
            if (eRingData.numElectron == eRingData.maxElectron) {eRingData.toggleFull();}
            numElectron++;

            // get new angle between electrons and scale of electron ring
            float angleBetween = 360f / (float) (newElectronIndex + 1);
            // Debug.Log(angleBetween);
            float radians = angleBetween * (float)Math.PI / 180f;
            float scale = eRing.transform.localScale.x;

            // calculate new positions of electrons
            for (float i = 0; i < 2*Math.PI; i += radians)
            {
                // newPositions.Add(new Vector3((float)Math.Cos(i), (float)Math.Sin(i), 0) * scale);
                Vector3 position = new Vector3((float)Math.Cos(i), (float)Math.Sin(i), 0) * scale;
                position = eRing.transform.rotation * position;
                position += ORIGIN_POINT; // Offset by ORIGIN_POINT
                newPositions.Add(position);
            }

            // set new positions for existing electrons
            for (int i = 0; i < newElectronIndex; i++)
            {
                eRingData.electronReferences[i].transform.position = newPositions[i];
            }

            // instantiate new electron
            GameObject newElectron = Instantiate(electronPrefab, newPositions[newElectronIndex], Quaternion.identity);
            newElectron.transform.parent = eRing.transform;
            newElectron.transform.name = $"Ring_{whereToAdd}_Electron_{newElectronIndex}";
            eRingData.electronReferences.Add(newElectron);

            eRingData.electronPositions = newPositions;
        }

        public void RemoveElectron()
        {
            // find which electron ring to remove electron from
            int whereToRemove = -1;
            for (int i = electronRings.Count - 1; i >= 0; i--)
            {
                if (electronRings[i].GetComponent<ElectronRing>().numElectron > 0)
                {
                    whereToRemove = i;
                    break;
                }
            }

            // NO MORE ELECTRONS!!!
            if (whereToRemove == -1)
            {
                EmitWarning("No Electron particles to remove!");
                return;
            }

            // some variable declarations
            GameObject eRing = electronRings[whereToRemove];
            ElectronRing eRingData = eRing.GetComponent<ElectronRing>();
            List<Vector3> newPositions = new List<Vector3>();
            int newElectronIndex = eRingData.numElectron - 2;

            // update numElectron in electron ring
            eRingData.decNumElectron();
            numElectron--;

            if (eRingData.numElectron == 0)
            {
                Destroy(eRingData.electronReferences[0]);
                eRingData.electronPositions.RemoveAt(0);
                eRingData.electronReferences.RemoveAt(0);
                eRingData.toggleActive();

                eRing.SetActive(false);
                return;
            }
            else
            {
                Destroy(eRingData.electronReferences[newElectronIndex+1]);
                eRingData.electronPositions.RemoveAt(newElectronIndex+1);
                eRingData.electronReferences.RemoveAt(newElectronIndex+1);
                if (eRingData.full) {eRingData.toggleFull();}
            }

            // get new angle between electrons and scale of electron ring
            float angleBetween = 360f / (float) (newElectronIndex + 1);
            // Debug.Log(angleBetween);
            float radians = angleBetween * (float)Math.PI / 180f;
            float scale = eRing.transform.localScale.x;

            // calculate new positions of electrons
            for (float i = 0; i < 2*Math.PI; i += radians)
            {
                // newPositions.Add(new Vector3((float)Math.Cos(i), (float)Math.Sin(i), 0) * scale);
                Vector3 position = new Vector3((float)Math.Cos(i), (float)Math.Sin(i), 0) * scale;
                position = eRing.transform.rotation * position;
                position += ORIGIN_POINT;
                newPositions.Add(position);
            }

            // set new positions for existing electrons
            for (int i = 0; i < newElectronIndex + 1; i++)
            {
                eRingData.electronReferences[i].transform.position = newPositions[i];
            }

            eRingData.electronPositions = newPositions;
        }

        void StartAllMovement()
        {
            foreach (Rigidbody rb in protonRigidbodies)
            {
                rb.isKinematic = false;
            }
            foreach (Rigidbody rb in neutronRigidbodies)
            {
                rb.isKinematic = false;
            }
        }

        void StopAllMovement()
        {
            foreach (Rigidbody rb in protonRigidbodies)
            {
                rb.isKinematic = true;
            }
            foreach (Rigidbody rb in neutronRigidbodies)
            {
                rb.isKinematic = true;
            }
        }

        void UpdateTallyCount()
        {
            numProtonUI.text = numProton.ToString();
            numNeutronUI.text = numNeutron.ToString();
            numElectronUI.text = numElectron.ToString();
        }
    

        void EmitWarning(string message)
        {
            OnWarningRaised?.Invoke(message);
        }

        void EmitParticleChanges()
        {
            OnParticlesChanged?.Invoke(numProton, numNeutron, numElectron);
        }

        #region Add / Remove in increments functions
        public void AddMultipleElectrons()
        {
            int electronsAdded = 0;

            for (; electronsAdded < addRemoveIncrement; electronsAdded++)
            {
                if (!CanAddElectron())
                    break;

                AddElectron();
            }

            EmitParticleChanges();

            if (electronsAdded < addRemoveIncrement)
            {
                EmitWarning($"Can't add more electrons. Added {electronsAdded} out of {addRemoveIncrement} requested.");
            }
            else
            {
                EmitWarning("");
            }

        }

        private bool CanAddElectron()
        {
            for (int i = 0; i < electronRings.Count; i++)
            {
                if (!electronRings[i].GetComponent<ElectronRing>().full)
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveMultipleElectrons()
        {
            int electronsRemoved = 0;
            for (; electronsRemoved < addRemoveIncrement; electronsRemoved++)
            {
                if (!CanRemoveElectron())
                    break;
                RemoveElectron();
            }
            EmitParticleChanges();
            if (electronsRemoved < addRemoveIncrement)
            {
                EmitWarning($"Can't remove more electrons. Removed {electronsRemoved} out of {addRemoveIncrement} requested.");
            }
            else
            {
                EmitWarning("");
            }
        }

        private bool CanRemoveElectron()
        {
            return numElectron > 0;
        }

        public void AddMultipleParticles(string type)
        {
            int particlesAdded = 0;
            for (; particlesAdded < addRemoveIncrement; particlesAdded++)
            {
                if (!CanAddParticle(type))
                    break;
                AddParticle(type);
            }
            EmitParticleChanges();
            if (particlesAdded < addRemoveIncrement)
            {
                EmitWarning($"Can't add more {type}s. Added {particlesAdded} out of {addRemoveIncrement} requested.");
            }
            else
            {
                EmitWarning("");
            }
        }

        public void RemoveMultipleParticles(string type)
        {
            int particlesRemoved = 0;
            for (; particlesRemoved < addRemoveIncrement; particlesRemoved++)
            {
                if (!CanRemoveParticle(type))
                    break;
                RemoveParticle(type);
            }
            EmitParticleChanges();
            if (particlesRemoved < addRemoveIncrement)
            {
                EmitWarning($"Can't remove more {type} particles. Removed {particlesRemoved} out of {addRemoveIncrement} requested.");
            }
            else
            {
                EmitWarning("");
            }
        }

        private bool CanRemoveParticle(string type)
        {
            if (type == "Neutron")
                return neutronParticles.Count > 0;
            else if (type == "Proton")
                return protonParticles.Count > 0;
            else
                return false;
        }

        private bool CanAddParticle(string type)
        {
            const int MaxParticlesAllowed = 300;
            if (type == "Neutron")
                return neutronParticles.Count < MaxParticlesAllowed;
            else if (type == "Proton")
                return protonParticles.Count < MaxParticlesAllowed;
            else
                return false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            
        }

        public void SetIncrement()
        {
            int[] incrementValues = { 1, 5, 10 };
            currentIncrementIndex = (currentIncrementIndex + 1) % incrementValues.Length;
            addRemoveIncrement = incrementValues[currentIncrementIndex];
            numAddRemoveIncrement.text = addRemoveIncrement.ToString();
        }
        #endregion

        private void EmitAllDefault()
        {
            EmitParticleChanges();
            EmitWarning("");
        }
    }
}