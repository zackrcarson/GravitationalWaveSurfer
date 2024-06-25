using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TimerButtonHandler : MonoBehaviour
{
    [SerializeField]
    private TimeSpeedManager timeSpeedManager;
    private float increment = 1f;

    public void IncreaseScale() => timeSpeedManager.Scale += increment;

    public void DecreaseScale() => timeSpeedManager.Scale -= increment;
}
