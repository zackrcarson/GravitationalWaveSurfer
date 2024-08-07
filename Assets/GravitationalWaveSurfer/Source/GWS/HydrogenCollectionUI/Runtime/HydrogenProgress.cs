using System;
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
        public static HydrogenProgress Instance { get; private set; }

        [SerializeField] 
        private ParticleInventory particleInventory;

        [SerializeField] 
        private ParticleInventoryEventChannel particleInventoryEventChannel;
        
        /// <summary>
        /// Is changed throughout the game for different scales
        /// </summary>
        public Slider hydrogenSlider;

        [SerializeField]
        public RectTransform whiteDwarfTick;

        [SerializeField]
        public RectTransform neutronStarTick;

        [SerializeField]
        public RectTransform blackHoleTick;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);    
        }

        private void OnEnable()
        {
            particleInventoryEventChannel.OnHydrogenCountChanged += UpdateProgress;
            hydrogenSlider.maxValue = 1;
        }

        private void OnDisable()
        {
            particleInventoryEventChannel.OnHydrogenCountChanged -= UpdateProgress;
        }

        private void Start()
        {
            hydrogenSlider.value = 0;
            // PositionTick(blackHoleTick, (float)HydrogenTracker.NEUTRON_STAR_THRESHOLD);
            // PositionTick(neutronStarTick, (float)HydrogenTracker.WHITE_DWARF_THRESHOLD);
            // PositionTick(whiteDwarfTick, (float)HydrogenTracker.NOTHING_HAPPENS_THRESHOLD);
        }

        private void PositionTick(RectTransform tick, float threshold)
        {
            float sliderWidth = hydrogenSlider.GetComponent<RectTransform>().rect.width;
            float normalizedPosition = threshold / (float)HydrogenTracker.NEUTRON_STAR_THRESHOLD;
            const float hardCodedValue = 875; // TODO - i.e. 1750 / 2 but what does this do?
            float tickPositionX = (normalizedPosition * sliderWidth) - hardCodedValue;
            tick.anchoredPosition = new Vector2(tickPositionX, tick.anchoredPosition.y);
        }

        /// <summary>
        /// Updates the slider; uses logarithmic scale
        /// </summary>
        /// <param name="amount"></param>
        private void UpdateProgress(double amount)
        {
            // Debug.Log(Math.Floor(Math.Log10(particleInventory.HydrogenCount)));
            
            // determines which capacity is the proper one for current hydrogen count
            int capacityIndex = (amount == 0) ? 0 : (int) Math.Floor(Math.Log10(particleInventory.HydrogenCount) / 10);
            double curCapacity = HydrogenTracker.HYDROGEN_CAPACITY[capacityIndex];

            // changes slider based on the logarithmic scale
            float sliderPosition = (float) (Math.Log10(particleInventory.HydrogenCount) / Math.Log10(curCapacity));
            hydrogenSlider.value = sliderPosition;
        }

        /// <summary>
        /// Accessed by HydrogenManager.cs to change the progress bar when needed
        /// </summary>
        /// <param name="newSlider">Slider component</param>
        public void ChangeProgressBar(Slider newSlider)
        {
            hydrogenSlider = newSlider;
        }
    }
}
