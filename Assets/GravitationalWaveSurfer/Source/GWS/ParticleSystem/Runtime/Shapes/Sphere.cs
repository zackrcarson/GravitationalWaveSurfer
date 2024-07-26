using UnityEngine;

namespace GWS.ParticleSystem.Runtime.Shapes
{
    [System.Serializable]
    public class Sphere: IShape
    {
        public Vector3 ConvertToShapeDirection(Vector3 direction)
        {
            return direction;
        }
    }
}