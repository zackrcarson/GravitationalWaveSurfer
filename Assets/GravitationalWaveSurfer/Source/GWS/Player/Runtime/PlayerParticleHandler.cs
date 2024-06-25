using UnityEngine;
using GWS.Input.Runtime;

[RequireComponent(typeof(ParticleSystem))]
public class PlayerParticleHandler : MonoBehaviour
{
    /// <summary>
    /// The reference to the player's rigidbody.
    /// </summary>
    [SerializeField]
    private new Rigidbody rigidbody;

    /// <summary>
    /// The higher this number is, the less particles emit.
    /// </summary>
    [SerializeField]
    private float particleRateDenominator;

    /// <summary>
    /// The number used to clamp emission rate of particles.
    /// </summary>
    [SerializeField]
    private float maxParticles;

    [SerializeField]
    private ParticleSystem movementParticles;

    /// <summary>
    /// The angle which the particles will emit, based off player input.
    /// </summary>
    private Quaternion targetRotation = Quaternion.Euler(new Vector3(90, 0, 0));

    private void Start()
    {
        movementParticles.Play();
    }

    /// <summary>
    /// The point at which the particles will stop if the magnitude of the velocity is lower than this.
    /// </summary>
    [SerializeField]
    private float speedThreshold;

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            SetParticleRotation(new Vector3(90, 0, 0));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            SetParticleRotation(new Vector3(90, 180, 0));
        }
        else if (Input.GetKey(KeyCode.A))
        {
            SetParticleRotation(new Vector3(90, 270, 0));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            SetParticleRotation(new Vector3(90, 90, 0));
        }
        else
        {
            SetParticleRotation(new Vector3(0, 0, 0));
        }

        movementParticles.transform.rotation = Quaternion.Lerp(
            movementParticles.transform.rotation,
            targetRotation,
            Time.deltaTime * 50 // This is an arbitrary number that looks good
        );

        var emission = movementParticles.emission;

        var magnitude = rigidbody.velocity.magnitude;

        if (magnitude > speedThreshold)
        {
            SpawnMovementParticles(magnitude / particleRateDenominator);
        }
        else
        {
            emission.enabled = false;
        }
    }

    private void SpawnMovementParticles(float rate)
    {
        var emission = movementParticles.emission;
        emission.enabled = true;
        emission.rateOverTime = Mathf.Clamp(rate, 0, maxParticles);
    }

    private void SetParticleRotation(Vector3 rotation)
    {
        targetRotation = Quaternion.Euler(rotation);
    }
}
