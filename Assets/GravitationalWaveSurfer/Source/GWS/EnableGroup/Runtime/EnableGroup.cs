using UnityEngine;

namespace GWS.EnableGroup.Runtime
{
    /// <summary>
    /// Enables and disables a <see cref="Behaviour"/> collection based on <see cref="EnableGroupObserver"/> events. 
    /// </summary>
    public class EnableGroup: MonoBehaviour
    {
        [SerializeField] private EnableGroupObserver enableGroupObserver;
        [SerializeField] private Behaviour[] behaviours;

        private void OnEnable()
        {
            enableGroupObserver.onEnableRaised.OnEvent += EnableAllBehaviours;
            enableGroupObserver.onDisableRaised.OnEvent += DisableAllBehaviours;
        }
        
        private void OnDisable()
        {
            enableGroupObserver.onEnableRaised.OnEvent -= EnableAllBehaviours;
            enableGroupObserver.onDisableRaised.OnEvent -= DisableAllBehaviours;
        }

        private void EnableAllBehaviours()
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.enabled = true;
            }
        }
        
        private void DisableAllBehaviours()
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.enabled = false;
            }
        }
    }
}