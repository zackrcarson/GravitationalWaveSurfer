using System;
using System.Collections.Generic;
using UnityEngine;

using GWS.HydrogenCollectionUI;
using GWS.HydrogenCollectionUI.Runtime;
using UnityEngine.UI;
using TMPro;

namespace GWS.HydrogenCollection.Runtime
{
    /// <summary>
    /// HydrogenEater.cs Instance gets created multiple times for some reason <br/>
    /// This manager is guaranteed to only exist by itself so multiplier works properly <br/>
    /// Right now AddHydrogen() has some redundancy but doesn't affect much so keeping it this way. 
    /// </summary>
    public class HydrogenManager : MonoBehaviour
    {
        public static HydrogenManager Instance { get; private set; }

        [Header("Related objects")]
        [SerializeField] 
        private ParticleInventory particleInventory;
        
        public GameObject HydrogenProgressBars;
        public List<Transform> Bars;
        /// <summary>
        /// Mainly referenced by HydrogenText.cs so it doesn't have to keep track of <br/>
        /// the active progress bar, just uses this one 
        /// </summary>
        public TextMeshProUGUI CurrentProgressBarText;

        [Header("Multiplier")]
        public int multiplier;

        private void Awake() 
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);    
        }

        private void Start()
        {
            if (HydrogenProgressBars == null) Debug.LogWarning("Hydrogen progress bars object not set!!!");    
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    Transform bar = HydrogenProgressBars.transform.Find($"HydrogenProgressBar{i+1}");
                    Bars.Add(bar);
                }
            }
            for (int i = 1; i < 6; i++)
            {
                Bars[i].gameObject.SetActive(false);
            }
            Bars[0].gameObject.SetActive(true);
            CurrentProgressBarText = Bars[0].Find("HydrogenCounter").GetComponent<TextMeshProUGUI>();
        }

        public void ChangeMultiplier(int value)
        {
            multiplier = value;
        }

        /// <summary>
        /// USELESS wrapper function of HydrogenEater.AddHydrogen() <br/>
        /// Since I figured out why 'using GWS.XXX' doesn't work in some places <br/>
        /// Stupid .asmdef files 
        /// </summary>
        /// <param name="value"></param>
        public void AddHydrogen(double value)
        {
            double amount = value * Math.Pow(10, multiplier);

            int capacityIndexBefore = (particleInventory.HydrogenCount == 0) ? 0 : (int) Math.Floor(Math.Log10(particleInventory.HydrogenCount) / 10);
            int capacityIndexAfter = (particleInventory.HydrogenCount == 0) ? 0 : (int) Math.Floor(Math.Log10(particleInventory.HydrogenCount + amount) / 10);
            // Debug.Log($"HydrogenManager.AddHydrogen: {capacityIndexBefore} => {capacityIndexAfter}");

            // change the progress bar if necessary
            if (capacityIndexAfter != capacityIndexBefore)
            {
                Bars[capacityIndexBefore].gameObject.SetActive(false);
                Bars[capacityIndexAfter].gameObject.SetActive(true);
                Slider newSlider = Bars[capacityIndexAfter].GetComponent<Slider>();
                HydrogenProgress.Instance.ChangeProgressBar(newSlider);
                CurrentProgressBarText = Bars[capacityIndexAfter].Find("HydrogenCounter").GetComponent<TextMeshProUGUI>();
            }

            HydrogenEater.Instance.AddHydrogen(amount);
        }
    }
}