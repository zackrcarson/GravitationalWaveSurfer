using System;
using System.Linq;
using UnityEngine;
using StarOutcome = GWS.UI.Runtime.Outcome.Star;

namespace GWS.Gameplay
{
    /// <summary>
    /// Singleton that holds information on the current amount of collected hydrogen.
    /// </summary>
    public class HydrogenTracker: MonoBehaviour
    {
        [SerializeField]
        public static HydrogenTracker Instance { get; private set; }

        private int hydrogen = 0;

        public int Hydrogen
        {
            get { return hydrogen; }
            set { hydrogen = Mathf.Clamp(value, 0, HYDROGEN_CAPACITY); }
        }

        private KeyCode[] keycodes = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Q, KeyCode.Space };

        public static event Action<int> OnHydrogenChanged;

        public const int HYDROGEN_CAPACITY = 10_000;

        public const double SOLAR_MASS = 1000;

        // All of the following are in units of SOLAR_MASS
        public const double NOTHING_HAPPENS_THRESHOLD = 0.08;

        public const double WHITE_DWARF_THRESHOLD = 8;

        public const double NEUTRON_STAR_THRESHOLD = 20;

        public const double THRESHOLD_MAX = 20;

        private void OnEnable()
        {
            PhaseOneTimer.TimeUp += EndPhaseOne;
        }

        private void OnDisable()
        {
            PhaseOneTimer.TimeUp -= EndPhaseOne;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void FixedUpdate()
        {
            keycodes.ToList().ForEach(key =>
            {
                if (Input.GetKey(key))
                {
                    SubtractHydrogen(1);
                }
            });
        }

        public void AddHydrogen(int amount)
        {
            Hydrogen += amount;
            OnHydrogenChanged?.Invoke(Hydrogen);
        }

        public void SubtractHydrogen(int amount)
        {
            Hydrogen -= amount;
            OnHydrogenChanged?.Invoke(Hydrogen);
        }

        private void EndPhaseOne()
        {
            StarOutcome outcome = StarOutcome.NothingHappens;
            double score = Hydrogen / HYDROGEN_CAPACITY;

            if (score >= WHITE_DWARF_THRESHOLD / THRESHOLD_MAX)
            {
                outcome = StarOutcome.WhiteDwarf;
            }
            else if (score >= NEUTRON_STAR_THRESHOLD / THRESHOLD_MAX)
            {
                outcome = StarOutcome.NeutronStar;
            }
            else if (score >= NEUTRON_STAR_THRESHOLD / THRESHOLD_MAX)
            {
                outcome = StarOutcome.BlackHole;
            }

            Debug.Log($"Outcome: {outcome}, Score: {score}");
            UnlockManager.Instance.UnlockOutcome(outcome);
            Debug.Log(PlayerPrefs.GetString(UnlockManager.UNLOCKED_OUTCOMES_STRING));
        }
    }

}
