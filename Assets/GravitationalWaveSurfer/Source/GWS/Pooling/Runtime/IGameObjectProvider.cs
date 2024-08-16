using UnityEngine;

namespace GWS.Pooling.Runtime
{
    /// <summary>
    /// Provides a <see cref="GameObject"/>
    /// </summary>
    public interface IGameObjectProvider
    {
        /// <summary>
        /// <inheritdoc cref="GameObject"/>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public GameObject gameObject { get; } 
    }
}