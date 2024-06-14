using UnityEngine;

namespace GWS.MeshDeformation.Runtime
{
    /// <summary>
    /// Utilities to modify the vertices of a mesh.
    /// </summary>
    public static class MeshDeformationUtility
    {
        /// <summary>
        /// Evaluates the new vertex (e.g. any point) position delta deforming to a certain point. 
        /// </summary>
        /// <param name="vertex">The mesh vertex to pinch.</param>
        /// <param name="point">The origin point of the force.</param>
        /// <param name="force">The magnitude of the force.</param>
        /// <returns>The new vertex position delta.</returns>
        public static Vector3 EvaluatePinchOrthogonalToNearestPoint(Vector3 vertex, Vector3 point, float force)
        {
            var pointToVertexDirection = Vector3.Normalize(point - vertex);
            var distance = Vector3.Distance(vertex, point);
            var attenuatedForce = force / (1 + distance);
            return pointToVertexDirection * attenuatedForce;
        }
    }
}