using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To be placed on hydrogen to make them jitter. 
/// </summary>
public class JitterEffect : MonoBehaviour
{
    [SerializeField]
    private float positionJitterIntensity = 0.1f;

    [SerializeField]
    private float rotationJitterIntensity = 1f;

    [SerializeField]
    private float jitterInterval = 0.1f;

    [SerializeField]
    private float lerpSpeed = 5f;

    [HideInInspector]
    public Vector3 originalPosition;

    private Quaternion originalRotation;

    [HideInInspector]
    public Vector3 targetPosition;

    private Quaternion targetRotation;

    private float jitterTimer;

    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        jitterTimer = 0f;

        targetPosition = originalPosition;
        targetRotation = originalRotation;
    }

    void FixedUpdate()
    {
        jitterTimer += Time.deltaTime;

        if (jitterTimer >= jitterInterval)
        {
            jitterTimer = 0f;

            targetPosition = originalPosition + Random.insideUnitSphere * positionJitterIntensity;
            targetRotation = originalRotation * Quaternion.Euler(
                Random.Range(-rotationJitterIntensity, rotationJitterIntensity),
                Random.Range(-rotationJitterIntensity, rotationJitterIntensity),
                Random.Range(-rotationJitterIntensity, rotationJitterIntensity)
            );
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * lerpSpeed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * lerpSpeed);
    }
}
