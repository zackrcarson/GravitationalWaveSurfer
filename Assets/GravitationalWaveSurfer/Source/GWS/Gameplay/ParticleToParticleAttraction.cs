using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWS.Gameplay
{
    public class ParticleToParticleAttraction : MonoBehaviour
    {
        //private float initialSpeed = 1f;

        //private float maxSpeed = 5f;

        //private float accelerationRate = 0.5f;

        //private Vector3 currentMidpoint;

        //private Transform otherParticle;

        //[SerializeField]
        //private GameObject parent;

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.CompareTag("Attractable"))
        //    {
        //        GetMidpoint(other.transform);
        //    }
        //}

        //private void GetMidpoint(Transform other)
        //{
        //    currentMidpoint = transform.position + other.position;
        //    otherParticle = other;
        //}

        //private void FixedUpdate()
        //{
        //    MoveTowardsMidpoint(this.transform);
        //    if (otherParticle != null) MoveTowardsMidpoint(otherParticle.transform);
        //}

        //private void MoveTowardsMidpoint(Transform obj)
        //{
        //    float currentSpeed = Mathf.Clamp(initialSpeed + accelerationRate * Time.time, initialSpeed, maxSpeed);
        //    float speed = currentSpeed * Time.deltaTime;

        //    obj.position = Vector3.MoveTowards(obj.position, currentMidpoint, speed);
        //    if (obj.TryGetComponent<JitterEffect>(out var jitterEffect))
        //    {
        //        jitterEffect.targetPosition = transform.position;
        //    }
        //}
    }
}