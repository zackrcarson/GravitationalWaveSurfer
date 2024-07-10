using UnityEngine;

namespace GWS.GeneralRelativity.Runtime
{
    /// <summary>
    /// Math class for General Relativity. 
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class GRPhysics // (i.e. GeneralRelativityPhysics) 
    {
        /// <summary>
        /// Kilograms to Ronnagrams conversion factor
        /// </summary>
        /// <remarks>The default mass unit for Unity is Kilograms.</remarks>
        public const double KilogramsToRonnagrams = 1e+27;
        
        /// <summary>
        /// Meters to Gigameters conversion factor
        /// </summary>
        /// <remarks>The default distance unit for Unity is meters.</remarks>
        public const double MetersToGigameters = 1e+9;
        
        /// <summary>
        /// The gravitational constant in N*m^2/kg^2
        /// </summary>
        public const double GravitationalConstant = 6.6743e-11;

        /// <summary>
        /// The speed of light in m/s
        /// </summary>
        public const double LightSpeed = 299_792_458;
        
        /// <summary>
        /// Evaluates the gravitational force between two masses in Newtons (Newtonian).
        /// </summary>
        /// <param name="mass">The first mass (kg).</param>
        /// <param name="otherMass">The second mass (kg).</param>
        /// <param name="distance">The distance between the two masses (m).</param>
        /// <param name="gravitationalConstant">The gravitational constant (n).</param>
        /// <returns></returns>
        /// <remarks>https://en.wikipedia.org/wiki/Newton%27s_law_of_universal_gravitation</remarks>
        public static double EvaluateGravitationalForceMagnitude(double mass, double otherMass, double distance, 
            double gravitationalConstant = GravitationalConstant)
        {   
            return gravitationalConstant * mass * otherMass / (distance * distance); 
        }

        /// <summary>
        /// Evaluates the direction of the gravitational force between two masses accounting for rotational forces. 
        /// </summary>
        /// <param name="sourcePosition">The attracted mass's position.</param>
        /// <param name="targetPosition">The attracting mass's position.</param>
        /// <param name="targetRotationDelta">The attracting mass's rotation delta.</param>
        /// <remarks>TODO - this isn't scientifically accurate at all</remarks>
        /// <returns></returns>
        public static Vector3 EvaluateGravitationalForceDirection(Vector3 sourcePosition, Vector3 targetPosition, 
            Quaternion targetRotationDelta)
        {
            return targetRotationDelta * Vector3.Normalize(targetPosition - sourcePosition);
        }
        
        // TODO - port over old functions as static functions such as the ones in GravitationalWave.cs
    }
}