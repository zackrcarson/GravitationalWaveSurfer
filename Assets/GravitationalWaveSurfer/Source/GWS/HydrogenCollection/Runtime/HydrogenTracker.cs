using System;
using System.Linq;
using GWS.Gameplay;
using GWS.Timing.Runtime;
using GWS.UI.Runtime;
using UnityEngine;

namespace GWS.HydrogenCollection.Runtime
{
    /// <summary>
    /// Manages a <see cref="ParticleInventory"/>
    /// </summary>
    public class HydrogenTracker: MonoBehaviour
    {
        [SerializeField]
        private ParticleInventory particleInventory; 

        // private int hydrogen = 0;
        //
        // public int Hydrogen
        // {
        //     get { return hydrogen; }
        //     set { hydrogen = Mathf.Clamp(value, 0, HYDROGEN_CAPACITY); }
        // }

        // private KeyCode[] keycodes = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Q, KeyCode.Space };

        public const int HYDROGEN_CAPACITY = 10_000;

        public const double SOLAR_MASS = 1.989e30;

        // All of the following are in units of SOLAR_MASS
        public const double NOTHING_HAPPENS_THRESHOLD = 0.08;

        public const double WHITE_DWARF_THRESHOLD = 8;

        public const double NEUTRON_STAR_THRESHOLD = 20;

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
            Outcome outcome = Outcome.NothingHappens;
            double score = particleInventory.HydrogenCount / HYDROGEN_CAPACITY;

            if (score >= NEUTRON_STAR_THRESHOLD / NEUTRON_STAR_THRESHOLD)
            {
                outcome = Outcome.BlackHole;
            }
            else if (score < NEUTRON_STAR_THRESHOLD / NEUTRON_STAR_THRESHOLD)
            {
                outcome = Outcome.NeutronStar;
            }
            else if (score < WHITE_DWARF_THRESHOLD / NEUTRON_STAR_THRESHOLD)
            {
                outcome = Outcome.WhiteDwarf;
            }
            else if (score < NOTHING_HAPPENS_THRESHOLD / NEUTRON_STAR_THRESHOLD)
            {
                outcome = Outcome.NothingHappens;
            }

            Debug.Log($"Outcome: {outcome}, Score: {score}");
            UnlockManager.Instance.UnlockOutcome(outcome);
        }
    }

}
