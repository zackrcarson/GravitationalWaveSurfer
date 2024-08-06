using GWS.HydrogenCollection.Runtime;
using GWS.UI.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using GWS.GeneralRelativitySimulation.Runtime;
using System;
using TMPro;

namespace GWS.AtomCreation.Runtime
{
    public class HeatManagement : MonoBehaviour
    {
        [SerializeField] 
        private ParticleInventoryEventChannel particleInventoryEventChannel;

        /// <summary>
        /// The temperature bar itself.
        /// </summary>
        [SerializeField] 
        private Slider bar;

        /// <summary>
        /// The rotating valve, visual effect only.
        /// </summary>
        [SerializeField]
        private RotationalBehavior valveRotation;
        private float currentValveRotation = 0f;

        /// <summary>
        /// Displays to the user the valid range of temperatures on the gauge.
        /// </summary>
        [SerializeField] 
        private RectTransform rangeIndicator;

        #region ui stuff
        [SerializeField]
        private TextMeshProUGUI elementName;

        [SerializeField]
        private TextMeshProUGUI elementMeV;
        #endregion

        #region audio stuff
        [SerializeField]
        private AudioSource constantAudioSource;

        [SerializeField]
        private AudioClip startCoolingSound;

        [SerializeField]
        private AudioClip endCoolingSound;

        [SerializeField]
        private AudioClip formationSound;
        #endregion

        private float temperature;
        private float maxTemperature = 9f;
        private float heatingRate = 2f;

        private float baseCoolingRate = 1.35f;
        private float coolingHeldDownTime = 0f;
        private float coolingAcceleration = 6.75f;

        private const KeyCode COOLING_KEY = KeyCode.R;
        private bool canCool = true;
        private bool isCooling = false;

        private ElementData element;
        private float elementProgress;
        private float elementFormationRate = 20f;
        private int currentElementIndex = 0;

        private void OnEnable()
        {
            particleInventoryEventChannel.OnHydrogenCountChanged += UpdateProgress;
        }

        private void OnDisable()
        {
            particleInventoryEventChannel.OnHydrogenCountChanged -= UpdateProgress;
        }

        private void Start()
        {
            temperature = 0;
            bar.minValue = 0;
            bar.maxValue = maxTemperature;
            bar.value = temperature;
            SetElement();
        }

        private void Update()
        {
            UpdateTemperature();
            HandleCoolingInput();
            CheckOverheating();
            CheckElementFormation();
            HandleValveRotation();
            print(elementProgress);
            bar.value = temperature;
        }

        private void UpdateTemperature()
        {
            if (!isCooling)
            {
                temperature += heatingRate * Time.deltaTime;
            }
        }

        private void HandleCoolingInput()
        {
            // Move to new input system
            if (UnityEngine.Input.GetKeyDown(COOLING_KEY) && canCool)
            {
                StartCooling();
            }
            else if (UnityEngine.Input.GetKey(COOLING_KEY) && canCool)
            {
                ContinueCooling();
            }
            else if (UnityEngine.Input.GetKeyUp(COOLING_KEY) && canCool)
            {
                StopCooling();
            }
        }

        private void StartCooling()
        {
            constantAudioSource.PlayOneShot(startCoolingSound, 0.25f);
            // StartCoroutine(CooldownRoutine());
        }

        private void ContinueCooling()
        {
            CoolDown();
            isCooling = true;
        }

        private void StopCooling()
        {
            isCooling = false;
            constantAudioSource.PlayOneShot(endCoolingSound, 0.5f);
            coolingHeldDownTime = 0;
        }

        private void CheckOverheating()
        {
            if (temperature >= maxTemperature)
            {
                Explode();
            }
        }

        private void CheckElementFormation()
        {
            if (IsWithinThreshold())
            {
                elementProgress += elementFormationRate * Time.deltaTime;
                if (elementProgress > element.duration)
                {
                    FormElement();
                    elementProgress = 0;
                }
            }
        }

        private bool IsWithinThreshold()
        {
            return temperature / maxTemperature <= element.upperThreshold && temperature / maxTemperature >= element.lowerThreshold;
        }
        
        private void HandleValveRotation()
        {
            if (isCooling)
            {
                // Lerp from current rotation to 360 degrees
                currentValveRotation = Mathf.MoveTowards(currentValveRotation, 1800f, 1000f * Time.deltaTime);
            }
            else
            {
                // Lerp from current rotation back to 0 degrees
                currentValveRotation = Mathf.MoveTowards(currentValveRotation, 0f, 3000f * Time.deltaTime);
            }

            valveRotation.rotationDelta.z = currentValveRotation;
        }

        private void SetThresholdRange(float lowerThreshold, float upperThreshold)
        {
            if (rangeIndicator == null || bar == null)
            {
                Debug.LogWarning("Range indicator, bar, or fill area is null. Cannot set threshold range.");
                return;
            }

            const int RANGE_INDICATOR_RECT_HEIGHT = 130;
            const int BOTTOM_TOP_OFFSET = 10;

            lowerThreshold = Mathf.Clamp01(lowerThreshold);
            upperThreshold = Mathf.Clamp01(upperThreshold);

            rangeIndicator.offsetMin = new Vector2((Mathf.Lerp(0, RANGE_INDICATOR_RECT_HEIGHT, lowerThreshold) + BOTTOM_TOP_OFFSET), rangeIndicator.offsetMin.y); // left
            rangeIndicator.offsetMax = new Vector2(((RANGE_INDICATOR_RECT_HEIGHT - (Mathf.Lerp(0, RANGE_INDICATOR_RECT_HEIGHT, upperThreshold)) + BOTTOM_TOP_OFFSET) * -1), rangeIndicator.offsetMax.y); // right
            
        }
        // This method can be used to adjust heating rate based on hydrogen count
        private void UpdateProgress(int amount)
        {
            //heatingRate = 1f + (amount / 100f); // Example: heating rate increases with more hydrogen
        }

        private void SetElement()
        {
            element = ElementInfo.Order[currentElementIndex];
            elementName.text = element.Element.ToString();
            elementMeV.text = $"{element.MeV} MeV";

            SetThresholdRange(element.lowerThreshold, element.upperThreshold);
        }

        private void CoolDown()
        {
            coolingHeldDownTime += Time.deltaTime;

            float currentCoolingRate = (float)(baseCoolingRate + (coolingAcceleration * Math.Pow(coolingHeldDownTime, 3))); // cubic rate of decrease to make it harder for the player to cool

            temperature -= currentCoolingRate * Time.deltaTime;
            temperature = Mathf.Clamp(temperature, 0, maxTemperature);
        }

       

        private void Explode()
        {
            Debug.Log("Ship exploded due to overheating!");
            ResetTemperature();
        }

        private void FormElement()
        {
            Debug.Log($"Formed element: {element}");
            currentElementIndex = Mathf.Min(currentElementIndex + 1, ElementInfo.Order.Length - 1);
            SetElement();
            constantAudioSource.PlayOneShot(formationSound);
        }

        private void ResetTemperature()
        {
            temperature = 0;
            //currentElementIndex = 0;
            SetElement();
        }
    }
}