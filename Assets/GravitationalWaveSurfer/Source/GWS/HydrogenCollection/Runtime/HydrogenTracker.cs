using System;
using System.Linq;
using GWS.Timing.Runtime;
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

        private const int HYDROGEN_MULTIPLIER = 1_000_000; // each hydrogen object can represent 1,000,000 kg?

        public const int HYDROGEN_CAPACITY = 10_000;

        public const double SOLAR_MASS = 1.989e30;

        // All of the following are in units of SOLAR_MASS
        public const double NOTHING_HAPPENS_THRESHOLD = 0.08;

        public const double WHITE_DWARF_THRESHOLD = 8;

        public const double NEUTRON_STAR_THRESHOLD = 20;

        public enum StarOutcome
        {
            NothingHappens,
            FusionBegins,
            WhiteDwarf,
            NeutronStar,
            BlackHole
        }

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
            StarOutcome outcome = StarOutcome.NothingHappens;
            double score = particleInventory.HydrogenCount * HYDROGEN_MULTIPLIER;

            if (score < NOTHING_HAPPENS_THRESHOLD * SOLAR_MASS)
            {
                outcome = StarOutcome.NothingHappens;
            }
            else if (score <= NOTHING_HAPPENS_THRESHOLD * SOLAR_MASS)
            {
                outcome = StarOutcome.FusionBegins;
            }
            else if (score < WHITE_DWARF_THRESHOLD * SOLAR_MASS)
            {
                outcome = StarOutcome.WhiteDwarf;
            }
            else if (score < NEUTRON_STAR_THRESHOLD * SOLAR_MASS)
            {
                outcome = StarOutcome.NeutronStar;
            }
            else if (score > NEUTRON_STAR_THRESHOLD * SOLAR_MASS)
            {
                outcome = StarOutcome.BlackHole;
            }

            Debug.Log($"Outcome: {outcome}, Score: {score}, Mass: {0.08f * SOLAR_MASS}");
        }
    }

}
