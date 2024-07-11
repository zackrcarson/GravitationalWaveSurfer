using GWS.HydrogenCollection.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace GWS.HydrogenCollectionUI.Runtime
{
    /// <summary>
    /// Draws the progress bar's fill and positions the ticks of each progress marker.
    /// </summary>
    public class HydrogenProgress: MonoBehaviour
    {
        [SerializeField] 
        private ParticleInventory particleInventory;

        [SerializeField] 
        private ParticleInventoryEventChannel particleInventoryEventChannel;
        
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
            particleInventoryEventChannel.OnHydrogenCountChanged += UpdateProgress;
            hydrogenSlider.maxValue = HydrogenTracker.HYDROGEN_CAPACITY;
        }

        private void OnDisable()
        {
            particleInventoryEventChannel.OnHydrogenCountChanged -= UpdateProgress;
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
            const float hardCodedValue = 875; // TODO - i.e. 1750 / 2 but what does this do?
            float tickPositionX = (normalizedPosition * sliderWidth) - hardCodedValue;
            tick.anchoredPosition = new Vector2(tickPositionX, tick.anchoredPosition.y);
        }

        private void UpdateProgress(int amount)
        {
            hydrogenSlider.value = particleInventory.HydrogenCount;
        }
    }
}
