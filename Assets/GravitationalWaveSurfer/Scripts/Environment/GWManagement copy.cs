// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class GWManagement : MonoBehaviour
// {
//     public Vector3 blackHolePosition; // Position of the black hole (source of gravitational wave)
//     public Vector3 waveEndpoint; // Endpoint of the gravitational wave
//     public float frequency = 1f; // Frequency of the gravitational wave
//     public float amplitude = 0.1f; // Amplitude of the gravitational wave
//     public float speedOfLight = 1f; // Speed of light in your simulation scale

//     public GameObject particles;
//     private Vector3 initialPosition;
//     private Vector3 projectedPoint;
//     private Vector3 waveDirection;
//     private Vector3 planeNormal;
//     private Quaternion rotationToPlane;
//     private float period;

//     public float planeSize = 4f;
//     public float planeThickness = 0.01f;

//     private void Start()
//     {
//         initialPosition = particle.transform.position;
//         period = 1f / frequency;
        
//         // Calculate wave direction and plane normal
//         waveDirection = (waveEndpoint - blackHolePosition).normalized;
//         planeNormal = -waveDirection;
        
//         // Project the initial position onto the gravitational wave line
//         projectedPoint = ProjectPointOnLine(initialPosition, blackHolePosition, waveEndpoint);
        
//         // Calculate the rotation from the XY plane to the perpendicular plane
//         rotationToPlane = Quaternion.FromToRotation(Vector3.forward, planeNormal);

//         CreatePlaneFromPointAndLine(initialPosition, blackHolePosition, waveDirection);
//     }

//     void Update()
//     {
//         float phase = (Time.time % period) / period;
//         ApplyPlusPolarization(phase);
//     }

//     private Vector3 ProjectPointOnLine(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
//     {
//         Vector3 directionVector = linePoint2 - linePoint1;
//         Vector3 pointVector = point - linePoint1;

//         float projection = Vector3.Dot(pointVector, directionVector) / directionVector.sqrMagnitude;

//         Vector3 projectedPoint = linePoint1 + projection * directionVector;

//         return projectedPoint;
//     }

//     void CreatePlaneFromPointAndLine(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
//     {
//         Vector3 directionVector = linePoint2 - linePoint1;
//         Vector3 projectedPoint = ProjectPointOnLine(point, linePoint1, linePoint2);
        
//         Vector3 projectedToPoint = point - projectedPoint;
//         Vector3 planeNormal = Vector3.Cross(directionVector, projectedToPoint).normalized;
        
//         // Create a new GameObject to represent the plane
//         GameObject planeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
//         planeObject.name = "PerpendicularPlane";
        
//         // Set the size and thickness of the plane
//         planeObject.transform.localScale = new Vector3(planeSize, planeThickness, planeSize);
        
//         // Position the plane at the projected point
//         planeObject.transform.position = projectedPoint;
        
//         // Orient the plane to face the plane normal
//         planeObject.transform.rotation = Quaternion.LookRotation(planeNormal, directionVector);
//     }

//     void ApplyPlusPolarization(float phase)
//     {
//         float s = 2 * Mathf.PI * phase;
        
//         // Calculate the propagation delay
//         float distanceAlongWave = Vector3.Dot(initialPosition - blackHolePosition, waveDirection);
//         float zDelay = distanceAlongWave / speedOfLight;
//         s -= 2 * Mathf.PI * (zDelay / period);

//         // Calculate the displacement vector from the projected point
//         Vector3 displacement = initialPosition - projectedPoint;

//         // Apply the plus polarization deformation in the XY plane
//         Vector3 deformedDisplacementXY = new Vector3(
//             displacement.x * (1 + amplitude * Mathf.Cos(s)),
//             displacement.y * (1 - amplitude * Mathf.Cos(s)),
//             0
//         );

//         // Rotate the deformed displacement to be on the perpendicular plane
//         Vector3 deformedDisplacement = rotationToPlane * deformedDisplacementXY;

//         // Calculate the final deformed position
//         Vector3 deformedPosition = projectedPoint + deformedDisplacement;

//         particle.transform.position = deformedPosition;
//     }

// }