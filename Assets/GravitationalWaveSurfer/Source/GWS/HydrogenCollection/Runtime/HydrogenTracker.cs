using System;
using System.Collections;
using System.Linq;
using Eflatun.SceneReference;
using GWS.Gameplay;
using GWS.Timing.Runtime;
using GWS.UI.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GWS.HydrogenCollection.Runtime
{
    /// <summary>
    /// Manages a <see cref="ParticleInventory"/>
    /// </summary>
    public class HydrogenTracker: MonoBehaviour
    {
        [SerializeField]
        private ParticleInventory particleInventory;

        [SerializeField] SceneReference blackHoleScene;
        [SerializeField] SceneReference neutronStarScene;
        [SerializeField] SceneReference whiteDwarfScene;
        [SerializeField] SceneReference nothingHappensScene;

        // private int hydrogen = 0;
        //
        // public int Hydrogen
        // {
        //     get { return hydrogen; }
        //     set { hydrogen = Mathf.Clamp(value, 0, HYDROGEN_CAPACITY); }
        // }

        // private KeyCode[] keycodes = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Q, KeyCode.Space };

        /// <summary>
        /// 6 capacities for 6 different progress bars with different scales
        /// </summary>
        /// <value></value>
        public static double[] HYDROGEN_CAPACITY = {1e10, 1e20, 1e30, 1e40, 1e50, 1e60};

        public const double NOTHING_HAPPENS_THRESHOLD = 0.08;

        public const double WHITE_DWARF_THRESHOLD = 8;

        public const double NEUTRON_STAR_THRESHOLD = 20;

        /// <summary>
        /// In seconds.
        /// </summary>
        private const float TIME_TO_CUTSCENE = 3f;

        private SceneReference sceneToPlay;

        private void Start()
        {
            particleInventory.HydrogenCount = 0;
        }

        private void OnEnable()
        {
            PhaseOneTimer.TimeUp += EndPhaseOne;
        }

        private void OnDisable()
        {
            PhaseOneTimer.TimeUp -= EndPhaseOne;
        }

        // I think we're not doing this anymore iirc
        // private void FixedUpdate()
        // {
        //     keycodes.ToList().ForEach(key =>
        //     {
        //         if (Input.GetKey(key))
        //         {
        //             SubtractHydrogen(1);
        //         }
        //     });
        // }

        private void EndPhaseOne()
        {
            Outcome outcome;
            double score = particleInventory.HydrogenCount / HYDROGEN_CAPACITY[5];
            double neutronStarRatio = NEUTRON_STAR_THRESHOLD / NEUTRON_STAR_THRESHOLD; // This equals 1
            double whiteDwarfRatio = WHITE_DWARF_THRESHOLD / NEUTRON_STAR_THRESHOLD;
            double nothingHappensRatio = NOTHING_HAPPENS_THRESHOLD / NEUTRON_STAR_THRESHOLD;

            if (score >= neutronStarRatio)
            {
                outcome = Outcome.BlackHole;
                sceneToPlay = blackHoleScene;
            }
            else if (score >= whiteDwarfRatio)
            {
                outcome = Outcome.NeutronStar;
                sceneToPlay = neutronStarScene;
            }
            else if (score >= nothingHappensRatio)
            {
                outcome = Outcome.WhiteDwarf;
                sceneToPlay = whiteDwarfScene;
            }
            else
            {
                outcome = Outcome.NothingHappens;
                sceneToPlay = nothingHappensScene;
            }

            Debug.Log($"Outcome: {outcome}, Score: {score}");
            UnlockManager.Instance.UnlockOutcome(outcome);
        }

        private IEnumerator TransitionToCutscene()
        {
            yield return new WaitForSeconds(4f);
            SceneManager.LoadScene(sceneToPlay.Name);
        }
    }

}
