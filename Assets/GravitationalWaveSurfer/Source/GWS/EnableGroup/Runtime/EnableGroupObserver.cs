using GWS.EventSystem.Runtime;
using UnityEngine;

namespace GWS.EnableGroup.Runtime
{
    [CreateAssetMenu(fileName = nameof(EnableGroupObserver), menuName = "ScriptableObjects/Enable Group Observer")]
    public class EnableGroupObserver : ScriptableObject
    {
        /// <summary>
        /// Callback on enabling the group. 
        /// </summary>
        public RaisableEvent onEnableRaised;
        
        /// <summary>
        /// Callback on disabling the group. 
        /// </summary>
        public RaisableEvent onDisableRaised; 
    }
}