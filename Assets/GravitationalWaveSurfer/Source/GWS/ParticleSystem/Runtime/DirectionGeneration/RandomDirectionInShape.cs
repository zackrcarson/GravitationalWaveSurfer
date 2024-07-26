using GWS.ParticleSystem.Runtime.Shapes;
using JetBrains.Annotations;
using UnityEngine;

namespace GWS.ParticleSystem.Runtime.DirectionGeneration
{
    /// <summary>
    /// Returns a random surface normal in a shape. 
    /// </summary>
    [System.Serializable]
    public class RandomDirectionInShape: IDirectionGenerator
    {
        [UsedImplicitly, SerializeReference, SubclassSelector]
        private IShape shape;

        public Vector3 GetDirection()
        {
            return shape.ConvertToShapeDirection(Random.insideUnitSphere);
        }
    }
}