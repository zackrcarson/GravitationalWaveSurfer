using System.Collections.Generic;
using GWS.MeshDeformation.Runtime;
using GWS.GeneralRelativity.Runtime;
using UnityEngine;

namespace GWS.GeneralRelativitySimulation.Runtime
{
    /// <summary>
    /// Represents a static, 3D slice of 4D space time, typically expressed as a some deforming plane.
    /// </summary>
    /// <remarks>
    /// Uses Ronnagrams instead of Kilograms to simulate. Unity uses Kilograms, so adjust your values accordingly,
    /// such as Rigidbody masses.
    /// </remarks>
    public class SpacelikeHypersurfaceVertexBehavior: MonoBehaviour
    {
        /// <summary>
        /// The simulated objects.
        /// </summary>
        [SerializeField]
        private SimulatedObjects simulatedObjects;
        
        /// <summary>
        /// The mesh that represents the Spacelike hypersurface, typically a plane.
        /// </summary>
        [SerializeField] 
        private MeshFilter meshFilter;
        
        /// <summary>
        /// The mass of every point along the hypersurface.
        /// </summary>
        /// <remarks>
        /// The space-time continuum doesn't have a mass (?), but this allows us to emulate it's warping
        /// by attracting each spacelike hypersurface to model it. SO we need to take this fictional mass into account.
        /// </remarks>
        [SerializeField, Min(0)] 
        private double mass;

        [SerializeField] 
        private float forceScale;

        /// <summary>
        /// The original vertices on <see cref="Awake"/>. 
        /// </summary>
        [SerializeField]
        private Vector3[] originalVertices;

        private void Awake()
        {
            originalVertices = meshFilter.mesh.vertices;
        }

        private void FixedUpdate()
        {
            Simulate();
        }

        private void Simulate()
        {
            var warpedVerticesSum = (Vector3[])originalVertices.Clone();
            
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var other in simulatedObjects.objects)
            {
                if (!CanSimulateObject(other)) continue;
                EvaluateWarpedVerticesNonAlloc(warpedVerticesSum, other);
            }
            
            meshFilter.mesh.vertices = warpedVerticesSum; 
        }

        private bool CanSimulateObject(ISimulatedObject other)
        {
            return other != null && other.Position != transform.position;
        }

        
        private void EvaluateWarpedVerticesNonAlloc(IList<Vector3> vertices, ISimulatedObject other)
        {
            var localScale = transform.localScale;
            var inverseLocalScale = new Vector3(1 / localScale.x, 1 / localScale.y, 1 / localScale.z);
            
            for (var i = 0; i < vertices.Count; i++)
            {
                var otherLocalPosition = transform.InverseTransformDirection(other.Position);
                var position = Vector3.Scale(vertices[i], localScale);
                var distance = Vector3.Distance(position, otherLocalPosition);
                var force = (float)(GRPhysics.EvaluateGravitationalForceMagnitude(mass, other.Mass, distance * GRPhysics.MetersToGigameters) 
                    * GRPhysics.KilogramsToRonnagrams / mass);
                var pinch = MeshDeformationUtility.EvaluatePinchOrthogonalToNearestPoint(position, otherLocalPosition, force * forceScale);
                
                // calculate clamped pinch in such a way that it doesn't go past other 
                var pinchDirection = Vector3.Normalize(pinch);
                var pinchPosition = position + Vector3.Scale(pinch, localScale);
                var halfSpaceTest = Vector3.Dot(pinchDirection,  otherLocalPosition - pinchPosition);
                var evaluatedPinch = pinch + (halfSpaceTest < 0 ? Vector3.Scale(pinchDirection * halfSpaceTest, inverseLocalScale): Vector3.zero);
                
                vertices[i] += evaluatedPinch;
                
                // TODO - visualization for rotation?
                // var rotation = Quaternion.SlerpUnclamped(Quaternion.identity, other.RotationDelta, gravitationalForce);
                // originalVertices[i] = rotation * originalVertices[i];
            }
        }
    }
}