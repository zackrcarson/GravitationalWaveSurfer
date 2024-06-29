using GWS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace GWS.UI.Runtime
{
    /// <summary>
    /// Draws the progress bar's fill and positions the ticks of each progress marker.
    /// </summary>
    public class HydrogenProgress: MonoBehaviour
    {
        [SerializeField]
        private Slider hydrogenSlider;

        [SerializeField]
        public RectTransform whiteDwarfTick;

        [SerializeField]
        public RectTransform neutronStarTick;

        [SerializeField]
        public RectTransform blackHoleTick;

        private void OnEnable()
        {
            HydrogenTracker.OnHydrogenChanged += UpdateProgress;
            hydrogenSlider.maxValue = HydrogenTracker.HYDROGEN_CAPACITY;
        }

        private void OnDisable()
        {
            HydrogenTracker.OnHydrogenChanged -= UpdateProgress;
        }

        private void Start()
        {
            hydrogenSlider.value = 0;
            PositionTick(blackHoleTick, (float)HydrogenTracker.NEUTRON_STAR_THRESHOLD);
            PositionTick(neutronStarTick, (float)HydrogenTracker.WHITE_DWARF_THRESHOLD);
            PositionTick(whiteDwarfTick, (float)HydrogenTracker.NOTHING_HAPPENS_THRESHOLD);
        }

        private void PositionTick(RectTransform tick, float threshold)
        {
            float sliderWidth = hydrogenSlider.GetComponent<RectTransform>().rect.width;
            float normalizedPosition = threshold / (float)HydrogenTracker.NEUTRON_STAR_THRESHOLD;
            float tickPositionX = (normalizedPosition * sliderWidth) - (1750 / 2);
            tick.anchoredPosition = new Vector2(tickPositionX, tick.anchoredPosition.y);
        }

        private void UpdateProgress(int amount)
        {
            hydrogenSlider.value = HydrogenTracker.Instance.Hydrogen;
        }
    }
}
