using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    // Config Parameters
    [SerializeField] float blackHoleRotationSpeed = 100f;

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            transform.Rotate(0, 0, blackHoleRotationSpeed * Time.unscaledDeltaTime);
        }
        else
        {
            transform.Rotate(0, 0, blackHoleRotationSpeed * Time.deltaTime);
        }
    }
}
